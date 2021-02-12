using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Functions;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Models;
using Utilities.MSACCO_SERVICE_SPEC;
using Utilities.PortalApplicationParams;


namespace MSacco_BLL.Controllers
{
  public class HomeController : Controller
  {
    private MobileWithdrawalsBLL _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
    private BulkSMSBLL _bulkSMSBLL = new BulkSMSBLL();

    // use of interfaces is the newer (preferred) way
    private IBL_SACCO _saccoBLL = new SaccoBLL();
    private IBL_BankTransfer _bankTransferBLL = new MsaccoBankTransferBLL(new SaccoBLL());
    public ActionResult Index()
    {
      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
      ViewBag.ActiveUserParams = userParams;
      ViewBag.BankTransferSpecs = new BankTransferServiceSpec(_bankTransferBLL.IsClientUsingCoretecFloat(userParams.ClientCorporateNo));
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
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      IMobileWithdrawals_SACCODB withdrawalRecord = _mobileWithdrawalsBLL.GetLatestWithdrawalForClient(clientCorporateNo);

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
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      BulkSMSBalance saccoBulkSMSBalance = _bulkSMSBLL.GetBulkSMSBalanceForClient(clientCorporateNo);

      return Json(new
      {
        amount = saccoBulkSMSBalance?.Balance,
        last_transaction_timestamp = saccoBulkSMSBalance?.Last_Updated
      }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [Authorize]
    public ActionResult GetBankTransferFloat(string clientCorporateNo)
    {

      // the flow:
      // get the float from the appropriate bll
      // return the json data
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      string err = string.Empty;
      try
      {
        // retrieve float only if client is using coretec float
        if (!_bankTransferBLL.IsClientUsingCoretecFloat(clientCorporateNo))
        {
          return null;
        }

        IClientBankTransferFloat clientBankTransferFloat = _bankTransferBLL.GetClientFloat(clientCorporateNo);

        return Json(new
        {
          amount = clientBankTransferFloat?.CurrentFloat,
          last_transaction_timestamp = clientBankTransferFloat?.FloatTransactionTimeStamp
        }, JsonRequestBehavior.AllowGet);
      }
      catch (ArgumentNullException)
      {
        // bank transfer service could not locate the client
        //log error to db
        err = $"The Bank Transfer service was unable to retrieve your record(s).{Environment.NewLine}Please notify Support immediately.";
        throw new HttpException(err);
      }
      catch (ArgumentException)
      {
        // client could not be located in client register
        //log error to db
        err = $"A problem occurred validating your identity.{Environment.NewLine}Please log out and try again.";
        throw new HttpException(err);
      }
    }

    #endregion
  }
}