using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.Transactions.Functions;
using MSacco_Dataspecs.Feature.Transactions.Models;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Utilities;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
    [RequireActiveUserSession]
    public class DepositsController : Controller
    {
        private readonly IBL_MPESADeposit _mpesaDepositsBLL = new MPESADepositsBLL();
        private readonly IBL_SACCO _saccoBLL = new SaccoBLL();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
            return View();
        }

        #region Others
        [HttpPost]
        [Authorize]
        [ValidateXToken]
        public ActionResult BenkiKuu(StatementFileViewModel statementFile, List<C2BStatementLines> lines)
        {
            ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
            if (userParams == null || statementFile == null || lines == null || !lines.Any())
            {
                return null;
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

                return null;
            }

            // submit the data to server
            List<IMPESADeposit> deposits = lines.ToList<IMPESADeposit>();
            IFile_MPESA_C2B_Statement c2BStatement = new MPESA_C2B_StatementFile(statementFile.ShortCode, deposits);

            bool isSubmitted = false;
            string actionMessage;
            try
            {
                isSubmitted = _mpesaDepositsBLL.SubmitTransaction(c2BStatement, User.Identity.Name, out actionMessage);
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

            return Json(APICommunication.Encrypt(
                  JsonConvert.SerializeObject(new
                  {
                      success = isSubmitted,
                      ex = actionMessage
                  }),
                  new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)),
                  JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
