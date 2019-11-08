using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_BLL.ViewModels;
using CallCenter_DAL;

namespace CallCenter_BLL.Controllers
{
  public class LinkMonitoringController : Controller
  {
    private LinkMonitoringBLL _linkMonitoringBLL = new LinkMonitoringBLL();
    // GET: LinkMonitoring
    public ActionResult Index()
    {
      return View();
    }

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetLinkInfoForClient(
      string clientCorporateNo, bool loadAll=false, int page=0, int size=0)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) && !loadAll)
      {
        return null;
      }
      dynamic linkInfo = null;

      if (!loadAll)
      {
        linkInfo = Mapper.Map<LinkMonitoring,LinkMonitoringViewModel>(
          _linkMonitoringBLL.GetLinkInfoForClient(clientCorporateNo));

        return Json(new
        {
          data = linkInfo
        }, JsonRequestBehavior.AllowGet);
      }
      else
      {
        // the flow:
        // 1. get the pagination parameters
        // 2. pass the pagination parameters to the bll function
        // 3. retrieve the data from the bll function


        //PaginationParameters pagingParams = new PaginationParameters(
        //  int.Parse(page), int.Parse(size), null);
        PaginationParameters pagingParams = new PaginationParameters(page, size, null);
        linkInfo = Mapper.Map<IEnumerable<LinkMonitoring>, IEnumerable<LinkMonitoringViewModel>>(
          _linkMonitoringBLL.GetLinkInfoForAllClients(out int lastPage, true, pagingParams)).ToArray();

        return Json(new
        {
          last_page = lastPage, // last page from the fetched recordset
          data = linkInfo
        }, JsonRequestBehavior.AllowGet);
      }
    }
    #endregion

  }
}
