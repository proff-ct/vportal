using CallCenter_Dataspecs.Functions;
using CallCenter_Dataspecs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utilities.PortalApplicationParams;
using VisibilityPortal_Dataspecs.Models;

namespace CallCenter_BLL.Controllers
{
  public class TIMSIController: Controller
  {
    private IBL_MSACCO_TIMSI_NumberChecker _msaccoTIMSIBLL = new MSaccoTIMSIBLL();
    // GET: MsaccoPlusNumberCheckers
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }

    #region Others
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
