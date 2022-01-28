using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Security;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
  [RequireActiveUserSession]
  public class UtilityPaymentsController : Controller
  {
    private MSaccoUtilityPaymentBLL _mSaccoUtilityPaymentBLL = new MSaccoUtilityPaymentBLL();

    // GET: UtilityPayments
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }


    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetUtilityPaymentRecords(string clientCorporateNo, int page, int size)
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

      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function

      int lastPage;

      //PaginationParameters pagingParams = new PaginationParameters(
      //  int.Parse(page), int.Parse(size), null);
      PaginationParameters pagingParams = new PaginationParameters(page, size, null);

      dynamic utilityPaymentRecords = _mSaccoUtilityPaymentBLL
        .GetMSaccoUtilityPaymentTrxListForClient(clientCorporateNo, out lastPage, true, pagingParams)
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = APICommunication.Encrypt(
          utilityPaymentRecords, new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
      }, JsonRequestBehavior.AllowGet);

    }

    [HttpGet]
    [Authorize]
    public ActionResult GetUtilityPaymentsFinancialSummaryForToday(string clientCorporateNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      IEnumerable<MSaccoUtilityPayment> withdrawals = _mSaccoUtilityPaymentBLL
        .GetClientMSaccoUtilityPaymentTrxListForToday(clientCorporateNo, out int lastPage)
        .Where(l => l.Status.Equals("Completed"))
        .OrderByDescending(l => l.Transaction_Date);

      return Json(new
      {
        last_transaction_timestamp = withdrawals.FirstOrDefault()?.Transaction_Date,
        sum = withdrawals?.Sum(w => w.Amount)
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
