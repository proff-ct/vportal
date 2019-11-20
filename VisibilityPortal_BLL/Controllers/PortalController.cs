using System.Linq;
using System.Web.Mvc;
using VisibilityPortal_BLL;
using VisibilityPortal_DAL;

namespace Visibility_Portal.Controllers
{
  public class PortalModuleController : Controller
  {
    private PortalModuleBLL _portalModuleBLL = new PortalModuleBLL();
    // GET: PortalModule
    public ActionResult Index()
    {
      return View();
    }

    // GET: PortalModule/Edit/5
    public ActionResult AddOrUpdate(int id = 0)
    {
      return (id == 0) ? View() : View("// get the view modeland return");
    }
    // POST: PortalModule/Edit/5
    [HttpPost]
    public ActionResult AddOrUpdate(PortalModule portalModule)
    {
      //_portalModuleBLL.Save(portalModule);
      return RedirectToAction("Index");
    }


    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetModuleList()
    {

      dynamic portalModules = _portalModuleBLL.GetModulesList().ToArray();

      return Json(new
      {
        data = portalModules
      }, JsonRequestBehavior.AllowGet);
    }
    #endregion

  }
}
