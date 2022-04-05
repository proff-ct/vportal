using Microsoft.AspNet.SignalR;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.Hubs;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.Transactions.Functions;
using MSacco_Dataspecs.Feature.Transactions.Models;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Security;
using MSacco_Dataspecs.SignalR_Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utilities;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
    [System.Web.Mvc.Authorize]
    [RequireActiveUserSession]
    public class DepositsController : Controller
    {
        private readonly IBL_MPESADeposit _mpesaDepositsBLL = new MPESADepositsBLL();
        private readonly IBL_SACCO _saccoBLL = new SaccoBLL();
        private readonly IHubContext<IMSACCOClientApp> _ctxMSACCOClientApp;

        public DepositsController() : this(GlobalHost.ConnectionManager.GetHubContext<IMSACCOClientApp>("msaccoClient"))
        {
        }

        public DepositsController(IHubContext<IMSACCOClientApp> ctxMSACCOClientHub)
        {
            _ctxMSACCOClientApp = ctxMSACCOClientHub;
        }

        // GET: Home
        public ActionResult Index()
        {
            ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
            return View();
        }

        #region Others
        [HttpGet]
        [System.Web.Mvc.Authorize]
        public ActionResult GetUploadedDepositFiles(string clientCorporateNo, int page, int size)
        {
            if (string.IsNullOrEmpty(clientCorporateNo))
            {
                return null;
            }
            ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
            if (userParams == null)
            {
                return Json(new { last_page = 0, data = "" }, JsonRequestBehavior.AllowGet);
            }
            // the flow:
            // 1. get the pagination parameters
            // 2. pass the pagination parameters to the bll function
            // 3. retrieve the data from the bll function


            PaginationParameters pagingParams = new PaginationParameters(page, size, null);

            dynamic records = _mpesaDepositsBLL
              .GetUploadedDepositRecordsForClient(clientCorporateNo, out int lastPage, true, pagingParams)
              .ToArray();

            return Json(new
            {
                last_page = lastPage, // last page from the fetched recordset
                data = APICommunication.Encrypt(records, new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [NoAsyncTimeout]
        [ValidateXToken]
        public async Task<ActionResult> BenkiKuu(StatementFileViewModel statementFile, List<C2BStatementLines> lines, CancellationToken cancel)
        {
            bool isSubmitted = false;
            string actionMessage = null;

            ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
            if (statementFile == null || lines == null || !lines.Any())
            {
                actionMessage = "No data received";
                goto exit_fn;
            }
            else if (string.IsNullOrEmpty(User.Identity.Name))
            {
                AppLogger.LogOperationException(
                    "DepositsController.BenkiKuu",
                    "User.Identity.Name is either null or empty for logged in user session",
                    new { statementFile, loggedInUser = User.Identity.Name },
                    new Exception("Missing Identity info"));

                actionMessage = $"Missing info.<br /> <br />Log out then log back in.";
                goto exit_fn;
            }

            // check if statementShortCode matches the c2bpaybill of the logged in user
            MSacco_Dataspecs.Models.ISACCO sacco = _saccoBLL.GetSaccoByUniqueParam(userParams.ClientCorporateNo);
            if (sacco.c2bPaybill != statementFile.ShortCode)
            {
                AppLogger.LogOperationException(
                    "DepositsController.BenkiKuu",
                    "Uploaded C2B statement DOES NOT MATCH C2B paybill for logged in user",
                    new { statementFile, loggedInUser = User.Identity.Name },
                    new Exception("C2B Mismatch!"));

                TransactionPortalServices.Services.EmailService tpEmailService = new TransactionPortalServices.Services.EmailService();
                tpEmailService.SendEmail(
                    $"C2B Mismatch for Uploaded Deposit File by {User.Identity.Name} for {sacco.saccoName_1}",
                    $"{User.Identity.Name} has been told to contact CoreTec support so that their details can be verified. We may need to update C2bPaybill in source information. Their C2B Paybill is {(string.IsNullOrEmpty(sacco.c2bPaybill) ? "not set in Source Info" : sacco.c2bPaybill)} and they have uploaded a statement with C2B of {statementFile.ShortCode} for organization {statementFile.Organization}",
                    "madote@coretec.co.ke"
                    );

                actionMessage = "Kindly contact CoreTec support to have your details verified.";
                goto exit_fn;
            }
            // submit the data to server
            List<IMPESADeposit> deposits = lines.ToList<IMPESADeposit>();
            IFile_MPESA_C2B_Statement c2BStatement = new MPESA_C2B_StatementFile(statementFile.ShortCode, deposits, User.Identity.Name)
            {
                Organization = statementFile.Organization,
                Operator = statementFile.Operator,
                StatementDate = statementFile.ReportDate,
                StatementFileName = statementFile.StatementFileName
            };
            // ensure upload proceeds even when user disconnects
            CancellationTokenSource s = CancellationTokenSource.CreateLinkedTokenSource(cancel, Response.ClientDisconnectedToken);
            s.Token.Register(() =>
            {
                AppLogger.LogEvent(
                    "DepositsConroller.BenkiKuu", $"File upload by {User.Identity.Name} for {statementFile.StatementFileName} has been cancelled. Performing retry. . . ", new { statementFile });
                Task.Run(() =>
                {

                    // process remaining deposits if present else notify user that uploads seem to have processed successfully
                    //if (_mpesaDepositsBLL.StatementInProcessing == null || !_mpesaDepositsBLL.StatementInProcessing.Deposits.Any())
                    //{
                    //    isSubmitted = true;
                    //    actionMessage = $"Successful upload reported for file {statementFile.StatementFileName}";

                    //    goto exit_aysnc;
                    //}

                    isSubmitted = _mpesaDepositsBLL.SubmitTransaction(c2BStatement, User.Identity.Name, out actionMessage);
                    actionMessage = actionMessage.Replace(Environment.NewLine, "<br /> <br />");

                    //exit_aysnc:
                    string asyncServerResponse = APICommunication.Encrypt(
                      JsonConvert.SerializeObject(new
                      {
                          success = isSubmitted,
                          ex = actionMessage,
                          title = "MSACCO Deposits"
                      }),
                      new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)
                      );

                    _ctxMSACCOClientApp.Clients.User(User.Identity.Name).serverMessage(
                        JsonConvert.SerializeObject(new
                        {
                            encKey = userParams.APIToken,
                            encSecret = userParams.APIAuthID
                        }),
                        asyncServerResponse
                        );

                });
            });
            try
            {
                Task<(bool, string actionMessage)> taskSubmit = Task.Run(() =>
                    (_mpesaDepositsBLL.SubmitTransaction(c2BStatement, User.Identity.Name, out actionMessage), actionMessage));

                do
                {
                    if (!cancel.IsCancellationRequested)
                    {
                        switch (taskSubmit.Status)
                        {
                            case TaskStatus.WaitingToRun:
                            case TaskStatus.Running:
                                (bool, string actionMessage) submitOutcome = await taskSubmit;
                                isSubmitted = submitOutcome.Item1;
                                actionMessage = submitOutcome.actionMessage.Replace(Environment.NewLine, "<br /> <br />");

                                break;
                            case TaskStatus.RanToCompletion:
                                isSubmitted = taskSubmit.Result.Item1;
                                actionMessage = taskSubmit.Result.actionMessage.Replace(Environment.NewLine, "<br /> <br />");

                                break;
                            default:
                                isSubmitted = false;
                                actionMessage = $"MSACCO failed to process file upload of {statementFile.StatementFileName}.";

                                break;
                        }

                        goto exit_fn;
                    }
                    else
                    {
                        isSubmitted = false;
                        actionMessage = $"File upload of {statementFile.StatementFileName} was cancelled.";

                        goto exit_fn;
                    }
                }
                while (!cancel.IsCancellationRequested);

                //isSubmitted = _mpesaDepositsBLL.SubmitTransaction(c2BStatement, User.Identity.Name, out actionMessage);
                //actionMessage = actionMessage.Replace(Environment.NewLine, "<br /> <br />");
            }
            catch (Exception ex)
            {
                actionMessage = "A problem occurred submitting the statement for processing. Kindly try again.";
                AppLogger.LogOperationException(
                  "DepositsController.BenkiKuu",
                  $"Exception while trying to submit C2B statement: {ex.Message}",
                  new { userParams, c2BStatement },
                  ex);
            }

        exit_fn:
            string serverResponse = APICommunication.Encrypt(
                  JsonConvert.SerializeObject(new
                  {
                      success = isSubmitted,
                      ex = actionMessage,
                      title = "MSACCO Deposits"
                  }),
                  new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)
                  );

            if (!Response.IsClientConnected || Response.ClientDisconnectedToken.IsCancellationRequested)
            {
                _ctxMSACCOClientApp.Clients.User(User.Identity.Name).serverMessage(
                    JsonConvert.SerializeObject(new
                    {
                        encKey = userParams.APIToken,
                        encSecret = userParams.APIAuthID
                    }),
                    serverResponse
                    );
            }
            return Json(serverResponse, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
