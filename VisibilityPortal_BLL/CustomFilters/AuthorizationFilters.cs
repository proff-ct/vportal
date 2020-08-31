using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using VisibilityPortal_BLL.Models;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL.CustomFilters
{
  public class AuthorizeSuperOrSystemAdmin : AuthorizeAttribute
  {
    public AuthorizeSuperOrSystemAdmin()
    {
      Roles = $"{PortalUserRoles.SystemRoles.SuperAdmin.ToString()}, {PortalUserRoles.SystemRoles.SystemAdmin.ToString()}";
    }
  }

  public class RequireSuperOrSystemAdmin : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      HttpContext ctx = HttpContext.Current;

      var controllerName = filterContext.RouteData.Values["controller"];
      var actionName = filterContext.RouteData.Values["action"];
      var message = String.Format(
        "{0} controller:{1} action:{2}", "onactionexecuting", controllerName, actionName);
      
      Debug.WriteLine(message, "Action Filter Log");
      if (!
        (ctx.User.IsInRole(PortalUserRoles.SystemRoles.SuperAdmin.ToString()) ||
         ctx.User.IsInRole(PortalUserRoles.SystemRoles.SystemAdmin.ToString())))
      {
        filterContext.Result = new RedirectResult("~/Error/Forbidden");
      }

      // check that we have the ActiveUser session variable if user is SystemAdmin
      if (ctx.User.IsInRole(PortalUserRoles.SystemRoles.SystemAdmin.ToString()))
      {
        if (ctx.Session["ActiveUserParams"] == null)
        {
          ctx.Session.Clear();
          ctx.Session.Abandon();
          ctx.GetOwinContext().Authentication.SignOut();
          filterContext.Result = new RedirectResult("~/Home/Index");
        }
      }

      base.OnActionExecuting(filterContext);
    }
  }
}
