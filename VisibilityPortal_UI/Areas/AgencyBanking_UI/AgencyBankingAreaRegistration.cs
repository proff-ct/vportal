using System.Web.Mvc;

namespace Visibility_Portal
{
    public class AgencyBankingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AgencyBanking_UI";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AgencyBanking_default",
                "AgencyBanking/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "AgencyBanking_BLL.Controllers" }
            );
        }
    }
}