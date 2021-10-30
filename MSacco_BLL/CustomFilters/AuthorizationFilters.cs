using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Utilities;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
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

  public class ValidateXToken : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      HttpContext ctx = HttpContext.Current;

      try
      {
        ValidateAjaxXSSToken(ctx.Request);
      }
      catch (Exception ex)
      {
        ctx.Session.Clear();
        ctx.Session.Abandon();
        ctx.GetOwinContext().Authentication.SignOut();

        filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Invalid request!");

        AppLogger.LogOperationException(
          "ValidateXToken", $"XToken validation error: {ex.Message}", new { ctx.Request.Headers }, ex);
      }

      base.OnActionExecuting(filterContext);
    }

    private void ValidateAjaxXSSToken(HttpRequest request)
    {
      string cookieToken = "";
      string formToken = "";

      string xToken = request.Headers.Get("XToken");
      if (!string.IsNullOrEmpty(xToken))
      {
        string[] tokens = xToken.Split(':');
        if (tokens.Length == 2)
        {
          cookieToken = tokens[0].Trim();
          formToken = tokens[1].Trim();
          AntiForgery.Validate(cookieToken, formToken);
        }
        else
        {
          throw new ArgumentException("Invalid token supplied!");
        }
      }
      else
      {
        throw new ArgumentException("Missing token header! Header 'XToken' not found!");
      }
    }
  }
}
