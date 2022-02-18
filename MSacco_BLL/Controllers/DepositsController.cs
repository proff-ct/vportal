using Microsoft.AspNet.SignalR;
using MSacco_BLL.CustomFilters;
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
        private readonly IHubContext<IMSACCOClientHub> _ctxMSACCOClientHub;

        public DepositsController() : this(GlobalHost.ConnectionManager.GetHubContext<IMSACCOClientHub>("msaccoClient"))
        {
        }

        public DepositsController(IHubContext<IMSACCOClientHub> ctxMSACCOClientHub)
        {
            _ctxMSACCOClientHub = ctxMSACCOClientHub;
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
        [ValidateXToken]
        public ActionResult BenkiKuu(StatementFileViewModel statementFile, List<C2BStatementLines> lines)
        {
            bool isSubmitted = false;
            string actionMessage;

            ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
            if (statementFile == null || lines == null || !lines.Any())
            {
                actionMessage = "No data received";
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
                    $"{User.Identity.Name} has been told to contact CoreTec support so that their details can be verified. We may need to update C2bPaybill in source information. Their C2B Paybill is {sacco.c2bPaybill ?? "not set in Source Info"} and they have uploaded a statement with C2B of {statementFile.ShortCode} for organization {statementFile.Organization}",
                    "madote@coretec.co.ke"
                    );

                actionMessage = "Kindly contact CoreTec support to have your details verified.";
                goto exit_fn;
            }

            // submit the data to server
            List<IMPESADeposit> deposits = lines.ToList<IMPESADeposit>();
            IFile_MPESA_C2B_Statement c2BStatement = new MPESA_C2B_StatementFile(statementFile.ShortCode, deposits)
            {
                Organization = statementFile.Organization,
                Operator = statementFile.Operator,
                StatementDate = statementFile.ReportDate
            };

            try
            {
                isSubmitted = _mpesaDepositsBLL.SubmitTransaction(c2BStatement, User.Identity.Name, out actionMessage);
                actionMessage = actionMessage.Replace(Environment.NewLine, "<br /> <br />");
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
                      ex = actionMessage
                  }),
                  new MSACCO_AES(userParams.APIAuthID, userParams.APIToken));

            if (!Response.IsClientConnected)
            {
                _ctxMSACCOClientHub.Clients.User(User.Identity.Name).Onyesha(
                    User.Identity.Name,
                    serverResponse,
                    new MSACCO_AES(userParams.APIAuthID, userParams.APIToken));
            }
            return Json(serverResponse, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
