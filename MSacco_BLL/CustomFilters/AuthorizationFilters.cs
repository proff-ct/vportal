using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using VisibilityPortal_Dataspecs.Models;

namespace MSacco_BLL.CustomFilters
{
  public class RequireActiveUserSession : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      HttpContext ctx = HttpContext.Current;

      object controllerName = filterContext.RouteData.Values["controller"];
      object actionName = filterContext.RouteData.Values["action"];
      string message = string.Format(
        "{0} controller:{1} action:{2}", "onactionexecuting", controllerName, actionName);

      Debug.WriteLine(message, "Action Filter Log");

      if (ctx.Session["ActiveUserParams"] == null)
      {
        ctx.Session.Clear();
        ctx.Session.Abandon();
        ctx.GetOwinContext().Authentication.SignOut();
        filterContext.Result = new RedirectResult("~/Home/Index");
      }

      base.OnActionExecuting(filterContext);
    }
  }

}
