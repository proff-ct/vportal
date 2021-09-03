using AutoMapper;
using MSacco_BLL.MSSQLOperators;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Functions;
using MSacco_Dataspecs.Feature.MsaccoWhitelisting.Functions;
using MSacco_Dataspecs.Feature.MsaccoWhitelisting.Models;
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
        _msaccoRegistrationBLL.GetMsaccoRegistrationRecordForClient(clientCorporateNo, memberTelephoneNo.Replace("-",""))
      };

      return Json(registeredMemberRecords, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    [Authorize]
    public ActionResult WhitelistMemberRecord(string clientCorporateNo, string memberTelephoneNo, string trustReason)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) || string.IsNullOrEmpty(memberTelephoneNo))
      {
        return null;
      }

      IMSACCO_WHITELISTING_ACTION_PARAMS actionData = new MsaccoWhitelistingBLL.WhitelistingParams
      {
        CustomerPhoneNo = memberTelephoneNo.Replace("-", ""),
        KYCNarration = trustReason,
        ActionUser = User.Identity.Name,
      };
      bool isWhitelisted = _msaccoWhitelistingBLL.WhitelistMember(
        clientCorporateNo, actionData, out string actionMessage);

      return Json(new
      {
        success = isWhitelisted,
        ex = actionMessage
      }, JsonRequestBehavior.AllowGet);
    }

    #endregion
  }
}
