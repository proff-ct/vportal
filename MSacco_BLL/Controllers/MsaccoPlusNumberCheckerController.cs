using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Functions;
using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Models;
using MSacco_Dataspecs.MSSQLOperators;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
  [RequireActiveUserSession]
  public class MsaccoPlusNumberCheckerController : Controller
  {
    private IBL_MsaccoPlusNumberChecker _msaccoPlusNumberCheckerBLL = new MsaccoPlusNumberCheckerBLL();
    // GET: MsaccoPlusNumberCheckers
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"]; 
      return View();
    }

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetMSACCOPlusDeviceRecord(string clientCorporateNo, string memberTelephoneNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }

      // the flow:
      // 1. get the member's record from db
      // 2. parse and pass the record to client
      dynamic[] registeredMemberDeviceRecord = {
        Mapper.Map<IMsaccoPlusNumberChecker, IMsaccoPlusNumberCheckerViewModel>(
        _msaccoPlusNumberCheckerBLL.GetMsaccoPlusRegisteredMemberDeviceRecordForClient(clientCorporateNo, memberTelephoneNo.Replace("-","")))
      };

      return Json(registeredMemberDeviceRecord, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    [Authorize]
    public ActionResult ResetMSACCOPlusDeviceRecord(string clientCorporateNo, string memberTelephoneNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }
      
      bool isReset = _msaccoPlusNumberCheckerBLL.ResetMsaccoPlusMemberDeviceForClient(
        clientCorporateNo, memberTelephoneNo, out string resetMessage);

      return Json(new
      {
        success = isReset,
        ex = resetMessage
      }, JsonRequestBehavior.AllowGet);
    }

    #endregion

  }
}
