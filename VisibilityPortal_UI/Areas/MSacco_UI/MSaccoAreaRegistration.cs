using System.Web.Mvc;

namespace Visibility_Portal
{
    public class MSaccoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MSacco_UI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MSacco_default",
                "MSacco/{controller}/{action}/{id}",
                new { controller="Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "MSacco_BLL.Controllers" }
            );
        }
    }
}