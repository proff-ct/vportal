using AutoMapper;
using MSacco_BLL.MSSQLOperators;
using MSacco_Dataspecs.Feature.MsaccoTIMSINumberChecker.Functions;
using MSacco_Dataspecs.Feature.MsaccoTIMSINumberChecker.Models;
using MSacco_Dataspecs.MSSQLOperators;
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
  [Authorize]
  [RequireSACCOAdmin]
  public class TIMSIController: Controller
  {
    private IBL_MSACCO_TIMSI_NumberChecker _msaccoTIMSIBLL = new MSaccoTIMSIBLL();
    private IBL_TIMSI_RESET_LOG _msaccoTIMSIResetLogBLL = new MSaccoTIMSIBLL.TIMSIResetDBLog();
    // GET: MsaccoPlusNumberCheckers
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }
    public ActionResult IMSIAuthenticatedRecords()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }
    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetResetDBLogRecords(string clientCorporateNo, int page, int size)
    {
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }
      IPaginationParameters pagingParams = new PaginationParameters(page, size, null);

      // the flow:
      // 1. get the member's record from db
      // 2. parse and pass the record to client
      dynamic[] dbResetLogRecords = 
        Mapper.Map<IEnumerable<IMSACCO_TIMSI_RESET_DB_LOG>, IEnumerable<IMSACCO_TIMSI_RESET_ViewModel>>(_msaccoTIMSIResetLogBLL.GetResetRecordsForClient(clientCorporateNo,  out int lastPage, true, pagingParams)).ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = dbResetLogRecords
      }, JsonRequestBehavior.AllowGet);
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetBlockedTIMSIRecord(string clientCorporateNo, string memberTelephoneNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }

      // the flow:
      // 1. get the member's record from db
      // 2. parse and pass the record to client
      dynamic[] registeredMemberDeviceRecord = {
        _msaccoTIMSIBLL.GetTIMSIMemberRecordForClient(clientCorporateNo, memberTelephoneNo.Replace("-",""))
      };

      return Json(registeredMemberDeviceRecord, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    [Authorize]
    public ActionResult ResetMSACCOTIMSIRecord(string clientCorporateNo, string memberTelephoneNo, string trustReason)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }

      IMSACCO_TIMSI_RESET_LOG_PARAMS resetData = new MSaccoTIMSIBLL.TIMSIResetParams
      {
        CustomerPhoneNo = memberTelephoneNo.Replace("-", ""),
        ResetNarration = trustReason,
        ActionUser = User.Identity.Name,
      };
      bool isReset = _msaccoTIMSIBLL.ResetTIMSIMemberRecordForClient(clientCorporateNo, resetData, out string resetMessage);

      return Json(new
      {
        success = isReset,
        ex = resetMessage
      }, JsonRequestBehavior.AllowGet);
    }

    #endregion
  }
}
