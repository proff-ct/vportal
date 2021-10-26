using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Functions;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Models;
using MSacco_Dataspecs.Feature.MsaccoWhitelisting.Functions;
using MSacco_Dataspecs.Feature.MsaccoWhitelisting.Models;
using MSacco_Dataspecs.MSSQLOperators;
using MSacco_Dataspecs.Security;
using Newtonsoft.Json;
using Utilities;
using Utilities.PortalApplicationParams;
using VisibilityPortal_BLL.CustomFilters;
using VisibilityPortal_Dataspecs.Models;

namespace MSacco_BLL.Controllers
{
  [Authorize]
  [RequireActiveUserSession]
  public class MSACCOWhitelistingController : Controller
  {
    private IBL_MSACCO_Whitelisting _msaccoWhitelistingBLL = new MsaccoWhitelistingBLL();
    private IBL_MsaccoRegistration _msaccoRegistrationBLL = new MsaccoRegistrationsBLL();
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
    public ActionResult GetMemberRecord(string clientCorporateNo, string memberTelephoneNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }

      // the flow:
      // 1. get the member's record from db
      // 2. parse and pass the record to client
      dynamic[] registeredMemberRecords = {
        Mapper.Map<IRouting_Table, IRegistrationRecordToWhitelistViewModel>(_msaccoRegistrationBLL.GetMsaccoRegistrationRecordForClient(clientCorporateNo, memberTelephoneNo.Replace("-","")))
      };

      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];

      return userParams == null ?
        Json(null, JsonRequestBehavior.AllowGet) :
        Json(
          APICommunication.Encrypt(
            JsonConvert.SerializeObject(registeredMemberRecords),
            new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)),
          JsonRequestBehavior.AllowGet);

    }
    [HttpPost]
    [Authorize]
    public ActionResult WhitelistMemberRecord(string clientCorporateNo, string memberTelephoneNo, string trustReason)
    {
      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
      if (userParams == null || string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }

      IMSACCO_WHITELISTING_ACTION_PARAMS actionData = new MsaccoWhitelistingBLL.WhitelistingParams
      {
        CustomerPhoneNo = memberTelephoneNo.Replace("-", ""),
        KYCNarration = trustReason,
        ActionUser = User.Identity.Name,
      };
      bool isWhitelisted = false;
      string actionMessage;
      try
      {
        isWhitelisted = _msaccoWhitelistingBLL.WhitelistMember(
        clientCorporateNo, actionData, out actionMessage);
      }
      catch (Exception ex)
      {
        actionMessage = "A problem occurred whitelisting the member. Kindly try again.";
        AppLogger.LogOperationException(
          "WhitelistMemberRecord", 
          $"Exception while trying to whitelist: {ex.Message}",
          new { clientCorporateNo, actionData },
          ex);
      }

      return Json(APICommunication.Encrypt(
            JsonConvert.SerializeObject(new
            {
              success = isWhitelisted,
              ex = actionMessage
            }),
            new MSACCO_AES(userParams.APIAuthID, userParams.APIToken)),
            JsonRequestBehavior.AllowGet);
    }

    #endregion
  }
}
