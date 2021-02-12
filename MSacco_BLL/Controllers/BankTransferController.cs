using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Functions;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
  [RequireActiveUserSession]
  public class BankTransferController : Controller
  {
    private IBL_BankTransfer _bankTransferBLL = new MsaccoBankTransferBLL(new SaccoBLL());

    // GET: IBankTransfer
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"];
      return View();
    }


    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetMSACCOBankTransferRecords(string clientCorporateNo, int page, int size)
    {
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }
      
      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function

      int lastPage;

      PaginationParameters pagingParams = new PaginationParameters(page, size, null);

      dynamic records = Mapper.Map<IEnumerable<IBankTransfer>,IEnumerable<IBankTransferViewModel>>(_bankTransferBLL
        .GetBankTransferRecordsForClient(clientCorporateNo, out lastPage, true, pagingParams))
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = records
      }, JsonRequestBehavior.AllowGet);

    }

    [HttpGet]
    [Authorize]
    public ActionResult GetBankTransferFinancialSummaryForToday(string clientCorporateNo)
    {
      if (string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      IEnumerable<IBankTransfer> records = _bankTransferBLL
        .GetClientBankTransferRecordsForToday(clientCorporateNo, out int lastPage)
        .Where(l => l.Status.Equals("Completed"))
        .OrderByDescending(l => l.TransactionDate);

      return Json(new
      {
        last_transaction_timestamp = records.FirstOrDefault()?.TransactionDate,
        sum = records?.Sum(w => w.Amount)
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
