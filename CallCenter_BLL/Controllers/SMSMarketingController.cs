﻿using AutoMapper;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_BLL.CustomFilters;
using CallCenter_DAL;
using CallCenter_Dataspecs.SMSMarketing.Functions;
using CallCenter_Dataspecs.SMSMarketing.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utilities;
using Utilities.PortalApplicationParams;
using CalLCenter_Dataspecs.Security;
using CallCenter_Dataspecs.MSACCOFinanceAuditor.Models;

namespace CallCenter_BLL.Controllers
{
    [Authorize]
    [RequireActiveUserSession]
    public class SMSMarketingController : Controller
    {
        private IBL_SMSMarketing _smsMarketingBLL;
        private IMSACCOFinanceAuditor _msaccoFinance;

        // GET: IBankTransfer
        public ActionResult Index()
        {
            ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
            return View();
        }


        #region Others
        [HttpGet]
        public ActionResult GetSMSMarketingRecords(string clientCorporateNo, int page, int size)
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

            _smsMarketingBLL = new SMSMarketingBLL(new SaccoBLL(), HttpContext.GetOwinContext());

            // the flow:
            // 1. get the pagination parameters
            // 2. pass the pagination parameters to the bll function
            // 3. retrieve the data from the bll function


            PaginationParameters pagingParams = new PaginationParameters(page, size, null);
            int lastPage = 0;
            dynamic records = null;

            try
            {
                records = _smsMarketingBLL
                   .GetUploadedSMSRecordsForClient(User.Identity.Name, out lastPage, true, pagingParams)
                   .ToArray();
            }
            catch (Exception ex)
            {
                AppLogger.LogOperationException("SMSController.GetPortalSMSRecords", "Exception while getting records", new { User.Identity.Name }, ex);
            }

            return Json(new
            {
                last_page = lastPage, // last page from the fetched recordset
                data = APICommunication.Encrypt(
                    JsonConvert.SerializeObject(records),
                    new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
            }, JsonRequestBehavior.AllowGet);



        }

        [HttpPost]
        [ValidateXToken]
        public ActionResult Taarifa(PortalSMSFileViewModel bulkSMSData)
        {
            bool isDispatched = false;
            string actionMessage;

            ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
            if (userParams == null || bulkSMSData == null || string.IsNullOrEmpty(bulkSMSData.Message) || string.IsNullOrEmpty(bulkSMSData.Corp) || bulkSMSData.RecipientList == null || !bulkSMSData.RecipientList.Any())
            {
                actionMessage = "Missing message data. Verify and retry";
                goto exit_fn;
            }

            _smsMarketingBLL = new SMSMarketingBLL(new SaccoBLL(), HttpContext.GetOwinContext());
            _msaccoFinance = new MSACCOFinanceAuditor();
            _msaccoFinance.Subscribe(_smsMarketingBLL);

            // validate contacts
            List<ISMSRecipient> smsRecipients = _smsMarketingBLL.GenerateRecipientList(bulkSMSData.RecipientList, out actionMessage);
            if (smsRecipients == null || !smsRecipients.Any())
            {
                actionMessage = string.IsNullOrEmpty(actionMessage) ? "No valid sms recipient phone numbers were found" : actionMessage;
                goto exit_fn;
            }

            // create sms file data
            IFile_PortalSMS smsFile = new PortalSMSFile(smsRecipients)
            {
                FileName = bulkSMSData.FileName,
                MessageBody = bulkSMSData.Message.Trim()
            };

            // send sms
            try
            {
                if (_smsMarketingBLL.DispatchSMS(bulkSMSData.Corp, smsFile, User.Identity.Name, out actionMessage))
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
                actionMessage = "An error occurred queuing the sms";

                AppLogger.LogOperationException(
                    "SMSMarketingController.Taarifa",
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
