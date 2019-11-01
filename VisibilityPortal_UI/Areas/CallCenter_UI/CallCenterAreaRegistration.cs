using System.Web.Mvc;

namespace Visibility_Portal
{
  public class CallCenterAreaRegistration : AreaRegistration
  {
    public override string AreaName => "CallCenter_UI";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "CallCenter_default",
          "CallCenter/{controller}/{action}/{id}",
          new { controller = "Home", action = "Index", id = UrlParameter.Optional },
          new string[] { "CallCenter_BLL.Controllers" }
      );
    }

    public static string AreaPathFromRoot => "~/Areas/CallCenter_UI";
  }
}