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
using MSacco_Dataspecs.Security;
using Newtonsoft.Json;
using Utilities.PortalApplicationParams;
using VisibilityPortal_BLL.CustomFilters;

namespace MSacco_BLL.Controllers
{
  [RequireSACCOAdmin]
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
      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
      if (userParams == null || string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
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

      return Json(
        APICommunication.Encrypt(
            JsonConvert.SerializeObject(registeredMemberDeviceRecord),
            new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)),
        JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    [Authorize]
    [ValidateXToken]
    public ActionResult ResetMSACCOPlusDeviceRecord(string clientCorporateNo, string memberTelephoneNo)
    {
      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
      if (userParams == null || string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }
      
      bool isReset = _msaccoPlusNumberCheckerBLL.ResetMsaccoPlusMemberDeviceForClient(
        clientCorporateNo, memberTelephoneNo, out string resetMessage);

      return Json(
        APICommunication.Encrypt(
            JsonConvert.SerializeObject(new
            {
              success = isReset,
              ex = resetMessage
            }),
            new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)),
        JsonRequestBehavior.AllowGet);
    }

    #endregion

  }
}
