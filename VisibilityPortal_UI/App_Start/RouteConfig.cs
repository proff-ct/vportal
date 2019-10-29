using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Visibility_Portal
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      // will uncomment the below when ready to implement attribute based routing
      //routes.MapMvcAttributeRoutes();
      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
          namespaces: new string[] { "VisibilityPortal_BLL.Controllers" }
      );
      // Since using ONE ASP.NET Identity to handle the users, this will
      // handle all calls to Account/* regardless of the module
      routes.MapRoute(
          name: "PortalAccount",
          url: "{controller}/{action}/{id}",
          defaults: null,
          namespaces: new string[] { "VisibilityPortal_BLL.Controllers" }
      );
    }
  }
}
