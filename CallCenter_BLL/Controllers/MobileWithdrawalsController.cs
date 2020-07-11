using System.Linq;
using System.Web.Mvc;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_Dataspecs.Functions;
using CallCenter_Dataspecs.MSSQLOperators;

namespace CallCenter_BLL.Controllers
{
  public class MobileWithdrawalsController : Controller
  {
    //private MobileWithdrawalsBLL _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
    private IBL_MobileWithdrawals _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
    // GET: MobileWithdrawals
    public ActionResult Index()
    {
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

      dynamic utilityPaymentRecords = _mobileWithdrawalsBLL
        .GetMobileWithdrawalsTrxListForClient(clientCorporateNo, out int lastPage, true, pagingParams)
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = utilityPaymentRecords
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
