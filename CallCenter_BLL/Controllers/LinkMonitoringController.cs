using System.Linq;
using System.Web.Mvc;
using CallCenter_BLL.MSSQLOperators;

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
    public ActionResult GetLinkInfoForClient(string clientCorporateNo, bool loadAll=false)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) && !loadAll)
      {
        return null;
      }
      dynamic linkInfo = null;

      if (!loadAll)
      {
        linkInfo = _linkMonitoringBLL.GetLinkInfoForClient(clientCorporateNo);
      }
      else
      {
        linkInfo = _linkMonitoringBLL.GetLinkInfoForAllClients();
      }
            
      return Json(new
      {
        data = linkInfo
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
