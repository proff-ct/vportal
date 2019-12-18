using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AgencyBanking_DAL;
using Dapper;
using Utilities.PortalApplicationParams;

namespace AgencyBanking_BLL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SelectSaccoController: Controller
    {
        
        public ActionResult Index()
        {
            ViewBag.saccolist = GetSaccoNameFromDB();
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public  ActionResult Index(string corporateno)
        {
            ActiveUserParams pActiveUserParams = new ActiveUserParams();
            pActiveUserParams.ClientCorporateNo = corporateno;
            Session[ActiveUserParams.SessionVaribleName()] = pActiveUserParams;
            return RedirectToRoute(new {controller = "Home", action = "Index"});
        }
        private  IEnumerable<SaccoModel> GetSaccoNameFromDB()
        {
            string connectionstring = "";
#if DEBUG
            connectionstring = @ConfigurationManager.ConnectionStrings["saccoDB_prod"].ConnectionString;
#else
connectionstring = @ConfigurationManager.ConnectionStrings["saccoDB_prod"].ConnectionString;
#endif
            var sql =
                "select [saccoid] as 'CorporateNo',[Name] as 'SaccoName'  from [saccosetup];";
            return DapperOrm.QueryGetList<SaccoModel>( sql);
        
    }
}
}
