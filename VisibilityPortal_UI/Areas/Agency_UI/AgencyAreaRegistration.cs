using System.Web.Mvc;

namespace Visibility_Portal

{
    public class AgencyAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Agency_UI";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Agency_default",
                "Agent/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "Agency_UI.Controllers" }
            );
        }
    }
}