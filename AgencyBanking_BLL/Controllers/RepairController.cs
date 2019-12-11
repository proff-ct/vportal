using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AgencyBanking_DAL;
using Dapper;

namespace AgencyBanking_BLL.Controllers
{
    [Authorize]
  public  class RepairController: Controller
    {
        /// <summary>
        /// TODO: Use corporate number in future for the saccoid of the currently logged in user
        /// </summary>
        /// <returns></returns>
        //GET: Repair/
        public ActionResult Index()
        {
            var model = new DeviceRepairModel();
            model.Saccoid = "To be Set";
            return View(model);
        }

        //Post: Repair/index
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(DeviceRepairModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckImeiExists(model.Imei))
                    {
                        new RepairBLL().NewRepair(model);
                        return Redirect("/");
                    }
                    ModelState.AddModelError("Imei", "The Imei was not found");
                    return View(model);

                }
            }
            catch (Exception e)
            {
                return Content(e.StackTrace);
            }
          
            return View(model);
        }

        private bool CheckImeiExists(string imei)
        {
            DynamicParameters par =  new DynamicParameters();
            par.Add("imei",imei);
            DeviceInfo device = DapperOrm.QueryGetSingle<DeviceInfo>("select * from deviceinfo where imei = @imei", par);
            return device != null && !string.IsNullOrEmpty(device.IMEI);
        }
    }
}
