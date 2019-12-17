using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AgencyBanking_BLL.util;

namespace AgencyBanking_BLL
{

    class CorporateNumberFilter : ActionFilterAttribute
    { 
        
        /// <summary>
    /// Checks whether the corporate number is set or is null
    /// Logic:
    ///     - if the system admin,regular user or admin has no corporate number logout then set the cooporate number
    ///     - Coretech admin are redirected to a page to select a sacco then the Corporate number is set
    
    /// </summary>
    /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!HttpContext.Current.User.IsInRole("SuperAdmin"))
            {
                if (string.IsNullOrEmpty(CurrentSacco.CorporateNo))
                {
                    HttpContext.Current.Session.Abandon();
                    FormsAuthentication.SignOut();
                    HttpContext.Current.Response.Redirect("/account");
                }
            }
            else
            {
                HttpContext.Current.Response.Redirect("/");
            }
        }
    }
}
