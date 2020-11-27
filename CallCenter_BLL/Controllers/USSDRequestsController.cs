using System.Linq;
using System.Web.Mvc;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_Dataspecs.Functions;
using CallCenter_Dataspecs.MSSQLOperators;
using CallCenter_Dataspecs.USSDRequests.Functions;

namespace CallCenter_BLL.Controllers
{
  public class USSDRequestsController : Controller
  {
    //private MobileWithdrawalsBLL _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
    private IBL_USSDRequestLog _ussdRequestsBLL = new USSDRequestLogBLL();
    // GET: MobileWithdrawals
    public ActionResult Index()
    {
      return View();
    }

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetUSSDRequestRecords(string clientCorporateNo, int page, int size)
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

      dynamic ussdRequestRecords = _ussdRequestsBLL
        .GetUSSDRequestLogsForClient(clientCorporateNo, out int lastPage, true, pagingParams)
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = ussdRequestRecords
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
