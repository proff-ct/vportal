using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AgencyBanking_BLL.Controllers
{
    class SummaryController: Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Content("hello");
        }
    }
}
