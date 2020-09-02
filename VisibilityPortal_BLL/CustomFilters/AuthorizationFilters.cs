using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;
using VisibilityPortal_DAL;
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
    private readonly string AuthorizedMSACCORole = string.Format(
      "{0}.{1}", PortalModule.MSaccoModule.moduleName, nameof(PortalUserRoles.SystemRoles.SystemAdmin));
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      HttpContext ctx = HttpContext.Current;
      ClaimsIdentity ci = ctx.User.Identity as ClaimsIdentity;
      string isModuleRoleEnabled = ci.Claims.FirstOrDefault(c => c.Type.Equals(AuthorizedMSACCORole))?.Value.ToLower();

      object controllerName = filterContext.RouteData.Values["controller"];
      object actionName = filterContext.RouteData.Values["action"];
      string message = string.Format(
        "{0} controller:{1} action:{2}", "onactionexecuting", controllerName, actionName);

      Debug.WriteLine(message, "Action Filter Log");
      if (!
        (ctx.User.IsInRole(PortalUserRoles.SystemRoles.SuperAdmin.ToString()) ||
         !string.IsNullOrEmpty(isModuleRoleEnabled) || isModuleRoleEnabled.Equals("true")))
      {
        filterContext.Result = new RedirectResult("~/Error/Forbidden");
      }

      // check that we have the ActiveUser session variable 
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

  public class RequireSACCOAdmin : ActionFilterAttribute
  {
    private readonly List<string> AuthorizedRoles = new List<string>
    {
      string.Format("{0}.{1}", PortalModule.MSaccoModule.moduleName, nameof(PortalUserRoles.SystemRoles.SystemAdmin)),
      string.Format("{0}.{1}", PortalModule.MSaccoModule.moduleName, nameof(PortalUserRoles.SystemRoles.Admin))
    };
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      HttpContext ctx = HttpContext.Current;

      object controllerName = filterContext.RouteData.Values["controller"];
      object actionName = filterContext.RouteData.Values["action"];
      string message = string.Format(
        "{0} controller:{1} action:{2}", "onactionexecuting", controllerName, actionName);

      Debug.WriteLine(message, "Action Filter Log");
      ClaimsIdentity ci = ctx.User.Identity as ClaimsIdentity;

      string isModuleRoleEnabled = ci.Claims.FirstOrDefault(c => AuthorizedRoles.Contains(c.Type))?.Value.ToLower();
      if (isModuleRoleEnabled.Equals("false"))
      {
        filterContext.Result = new RedirectResult("~/Error/Forbidden");
      }

      // check that we have the ActiveUser session variable set
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
