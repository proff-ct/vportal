using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CallCenterAutoMapper = CallCenter_BLL.Utilities.AutoMapper;
using PortalAutoMapper = VisibilityPortal_BLL.Utilities.AutoMapper;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using AutoMapper;

namespace Visibility_Portal
{
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      Database.SetInitializer(new PortalDatabaseInitializer());
      Mapper.Initialize(config => {
        CallCenterAutoMapper.Mappings.MapperConfigExpr.Invoke(config);
        PortalAutoMapper.Mappings.MapperConfigExpr.Invoke(config);
      });
    }
  }
}
