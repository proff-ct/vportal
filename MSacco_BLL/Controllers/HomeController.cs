using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSacco_DAL;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
  public class HomeController : Controller
  {
    private MobileWithdrawalsBLL _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
    private BulkSMSBLL _bulkSMSBLL = new BulkSMSBLL();
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetMPesaFloat(string clientCorporateNo)
    {


      // the flow:
      // get the float from the appropriate bll
      // return the json data
      if (string.IsNullOrEmpty(clientCorporateNo)) return null;

      MobileWithdrawals withdrawalRecord = _mobileWithdrawalsBLL.GetLatestWithdrawalForClient(clientCorporateNo);

      return Json(new
      {
        amount = withdrawalRecord?.MPESA_Float_Amount,
        last_transaction_timestamp = withdrawalRecord?.MPESA_DateTime
      }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [Authorize]
    public ActionResult GetBulkSMSFloat(string clientCorporateNo)
    {


      // the flow:
      // get the float from the appropriate bll
      // return the json data
      if (string.IsNullOrEmpty(clientCorporateNo)) return null;

      BulkSMSBalance saccoBulkSMSBalance = _bulkSMSBLL.GetBulkSMSBalanceForClient(clientCorporateNo);

      return Json(new
      {
        amount = saccoBulkSMSBalance?.Balance,
        last_transaction_timestamp = saccoBulkSMSBalance?.Last_Updated
      }, JsonRequestBehavior.AllowGet);
    }

      #endregion
    }
  }