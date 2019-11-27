using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VisibilityPortal_BLL.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      dynamic model = Session["ClientModuleUrls"];
      if (model != null) ViewBag.ClientModuleUrls = model;
      return model!=null ? View(model) : View();
      //return RedirectToAction("Login", "Account");
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }
  }
}