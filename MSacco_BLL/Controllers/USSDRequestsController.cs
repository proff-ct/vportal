﻿using AutoMapper;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_Dataspecs;
using MSacco_Dataspecs.Feature.USSDRequestLogging.Functions;
using MSacco_Dataspecs.MSSQLOperators;
using MSacco_Dataspecs.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utilities.PortalApplicationParams;
using VisibilityPortal_BLL.CustomFilters;
using VisibilityPortal_Dataspecs.Models;

namespace MSacco_BLL.Controllers
{
    [RequireActiveUserSession]
    public class USSDRequestsController : Controller
    {
        private IBL_USSDRequestLog _ussdRequestsBLL = new USSDRequestLogBLL();
        // GET: USSDRequestLogs
        public ActionResult Index()
        {
            ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
            return View();
        }

        #region Others
        [HttpGet]
        [Authorize]
        public ActionResult GetUSSDRequestRecords(string clientCorporateNo, int page, int size)
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
            IPaginationParameters pagingParams = new PaginationParameters(page, size, null);

            // the flow:
            // 1. get the member's record from db
            // 2. parse and pass the record to client
            dynamic[] ussdRequestRecords = _ussdRequestsBLL
              .GetUSSDRequestLogsForClient(clientCorporateNo, out int lastPage, true, pagingParams)
              .ToArray();

            return Json(new
            {
                last_page = lastPage, // last page from the fetched recordset
                data = APICommunication.Encrypt(
                        JsonConvert.SerializeObject(ussdRequestRecords),
                        new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Authorize]
        public ActionResult GetUSSDRequestLogsForMember(string clientCorporateNo, string memberTelephoneNo)
        {
            if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
            {
                return null;
            }
            ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
            if (userParams == null)
            {
                return null;
            }

            // the flow:
            // 1. get the member's record from db
            // 2. parse and pass the record to client
            dynamic[] registeredMemberDeviceRecord = _ussdRequestsBLL
              .GetMemberUSSDRequestLogForClient(clientCorporateNo, MSACCOToolbox.ParsePhoneNo(memberTelephoneNo))
              .ToArray();

            return Json(new
            {
                last_page = 1,
                data = APICommunication.Encrypt(
                        JsonConvert.SerializeObject(registeredMemberDeviceRecord),
                        new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
