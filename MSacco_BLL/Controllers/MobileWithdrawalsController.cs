using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Models;
using MSacco_Dataspecs.MSSQLOperators;

using Utilities.PortalApplicationParams;
using MSacco_Dataspecs.Security;

namespace MSacco_BLL.Controllers
{
  [RequireActiveUserSession]
  public class MobileWithdrawalsController : Controller 
  {
    private IBL_MobileWithdrawals _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
    // GET: MobileWithdrawals
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"]; 
      return View();
    }

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetMobileWithdrawalRecords(string clientCorporateNo, int page, int size)
    {
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function


      //PaginationParameters pagingParams = new PaginationParameters(
      //  int.Parse(page), int.Parse(size), null);
      IPaginationParameters pagingParams = new PaginationParameters(page, size, null);

      dynamic mobileWithdrawalRecords = _mobileWithdrawalsBLL
        .GetMobileWithdrawalsTrxListForClient(clientCorporateNo, out int lastPage, true, pagingParams)
        .Select(r => { if (r.Transaction_Date == null) r.Transaction_Date = r.Datetime; return r; }) // didn't want to use ForEach ext. method
        .ToArray();

      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
      if(userParams == null)
      {
        return Json(new { last_page = lastPage, data = ""}, JsonRequestBehavior.AllowGet);
      }

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = APICommunication.Encrypt(
          JsonConvert.SerializeObject(mobileWithdrawalRecords), 
          new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
    }, JsonRequestBehavior.AllowGet);

    }
    [HttpGet]
    [Authorize]
    public ActionResult GetMobileWithdrawalsFinancialSummaryForToday(string clientCorporateNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      IEnumerable<IMobileWithdrawals_SACCODB> withdrawals = _mobileWithdrawalsBLL
        .GetClientMobileWithdrawalsFinancialSummaryForToday(clientCorporateNo, out int lastPage)
        .Where(l => l.Status.Equals("Completed"))
        .OrderByDescending(l => l.Transaction_Date ?? l.Datetime);

      return Json(new
      {
        last_transaction_timestamp = withdrawals.FirstOrDefault().Transaction_Date?? withdrawals.FirstOrDefault().Datetime,
        sum = withdrawals.Sum(w => w.Amount)
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
