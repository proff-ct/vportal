using System.Web.Mvc;

namespace Visibility_Portal
{
    public class CallCenterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CallCenter_UI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CallCenter_default",
                "CallCenter/{controller}/{action}/{id}",
                new { controller = "Homes", action = "Index", id = UrlParameter.Optional },
                new string[] { "CallCenter_UI.Controllers" }
            );
        }
    }
}