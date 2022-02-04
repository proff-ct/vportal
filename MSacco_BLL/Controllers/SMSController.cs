using AutoMapper;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.PortalSMS.Functions;
using MSacco_Dataspecs.Feature.PortalSMS.Models;
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
    public class SMSController : Controller
    {
        private readonly IBL_PortalSMS _portalSMSBLL = new PortalSMSBLL(new SaccoBLL());

        // GET: IBankTransfer
        public ActionResult Index()
        {
            ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
            return View();
        }


        #region Others
        [HttpGet]
        [Authorize]
        public ActionResult GetPortalSMSRecords(string clientCorporateNo, int page, int size)
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

            dynamic records = _portalSMSBLL
                .GetUploadedSMSRecordsForClient(User.Identity.Name, out int lastPage, true, pagingParams)
                .ToArray();

            return Json(new
            {
                last_page = lastPage, // last page from the fetched recordset
                data = APICommunication.Encrypt(records, new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize]
        [ValidateXToken]
        public ActionResult Peperusha(PortalSMSFileViewModel bulkSMSData)
        {
            ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
            if (userParams == null || bulkSMSData == null || bulkSMSData.RecipientList == null || !bulkSMSData.RecipientList.Any())
            {
                return null;
            }

            bool isDispatched = false;

            // validate contacts
            List<ISMSRecipient> smsRecipients = _portalSMSBLL.GenerateRecipientList(bulkSMSData.RecipientList, out string actionMessage);
            if (smsRecipients == null || !smsRecipients.Any())
            {
                actionMessage = string.IsNullOrEmpty(actionMessage) ? "No valid sms recipient phone numbers were found" : actionMessage;
                goto exit_fn;
            }

            // send sms
            IFile_PortalSMS smsFile = new PortalSMSFile(smsRecipients)
            {
                FileName = bulkSMSData.FileName,
                MessageBody = bulkSMSData.Message
            };

            try
            {
                if (_portalSMSBLL.DispatchSMS(userParams.ClientCorporateNo, smsFile, User.Identity.Name, out actionMessage))
                {
                    actionMessage = string.IsNullOrEmpty(actionMessage) ? "SMS queued for sending" : actionMessage;
                    isDispatched = true;
                }
                else
                {
                    actionMessage = string.IsNullOrEmpty(actionMessage) ? "Failed to queue sms for sending" : actionMessage;
                }
            }
            catch (Exception ex)
            {
                actionMessage = "An error occurred sending the sms";

                AppLogger.LogOperationException(
                    "SMSController.Peperusha",
                    "Exception while sending sms",
                    new { smsFile, loggedBy = User.Identity.Name },
                    ex);
            }

        // return response to client
        exit_fn:
            return Json(APICommunication.Encrypt(
                  JsonConvert.SerializeObject(new
                  {
                      success = isDispatched,
                      ex = actionMessage
                  }),
                  new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)),
                  JsonRequestBehavior.AllowGet);

        }
        #endregion

    }
}
