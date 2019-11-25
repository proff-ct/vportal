using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VisibilityPortal_BLL.Controllers
{
  public class ErrorController : Controller
  {
    public ActionResult Forbidden()
    {

      return View("Unauthorized");
    }
  }
}
