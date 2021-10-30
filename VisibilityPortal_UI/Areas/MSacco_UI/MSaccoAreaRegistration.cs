using System.Web.Mvc;
using System.Web.Optimization;

namespace Visibility_Portal
{
  public class MSaccoAreaRegistration : AreaRegistration
  {
    public override string AreaName => "MSacco_UI";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "MSacco_default",
          "MSacco/{controller}/{action}/{id}",
          new { controller = "Home", action = "Index", id = UrlParameter.Optional },
          new string[] { "MSacco_BLL.Controllers" }
      );

      BundleTable.Bundles.Add(new ScriptBundle("~/msacco/bundles/scripts").Include(
        "~/Areas/MSacco_UI/plugins/pg_intl-tel-input-11.1.0/js/intlTelInput.min.js",
        "~/Areas/MSacco_UI/plugins/pg_intl-tel-input-11.1.0/js/utils.js"));

      BundleTable.Bundles.Add(new StyleBundle("~/msacco/bundles/styles").Include(
        "~/Areas/MSacco_UI/plugins/pg_intl-tel-input-11.1.0/css/intlTelInput.css",
        "~/Areas/MSacco_UI/Content/msacco-global.css"));
    }

    public static string AreaPathFromRoot => "~/Areas/MSacco_UI";
  }
}