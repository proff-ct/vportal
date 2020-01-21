using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
  [RequireActiveUserSession]
  public class AirtimeTopupsController : Controller
  {
    private MSaccoAirtimeTopupBLL _mSaccoAirtimeTopupBLL = new MSaccoAirtimeTopupBLL();

    // GET: AirtimeTopups
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetAirtimeTopupRecords(string clientCorporateNo, int page, int size)
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
      PaginationParameters pagingParams = new PaginationParameters(page, size, null);

      dynamic airtimeTopupRecords = _mSaccoAirtimeTopupBLL
        .GetMSaccoAirtimeTopupTrxListForClient(clientCorporateNo, out int lastPage, true, pagingParams)
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = airtimeTopupRecords
      }, JsonRequestBehavior.AllowGet);

    }[HttpGet]
    [Authorize]
    public ActionResult GetAirtimeTopupsFinancialSummaryForToday(string clientCorporateNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      IEnumerable<MSaccoAirtimeTopup> airtimeTopupRecords = _mSaccoAirtimeTopupBLL
        .GetClientMSaccoAirtimeTopupTrxListForToday(clientCorporateNo, out int lastPage)
        .Where(l => l.Status.Equals("Completed"))
        .OrderByDescending(l => l.Transaction_Date)
        .ToArray();

      return Json(new
      {
        last_transaction_timestamp = airtimeTopupRecords.FirstOrDefault()?.Transaction_Date,
        sum = airtimeTopupRecords?.Sum(w => w.Amount)
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
