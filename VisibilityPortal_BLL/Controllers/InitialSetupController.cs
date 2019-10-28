using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VisibilityPortal_BLL.Controllers
{
  
  public class InitialSetupController : Controller
  {
    //
    // GET: /InitialSetup/SetPassphrase
    [AllowAnonymous]
    public ActionResult SetPassphrase()
    {
      return View();
    }
    //
    // POST: /InitialSetup/SetPassphrase
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult SetPassphrase(FormCollection form)
    {


      return View();
    }
  }
}