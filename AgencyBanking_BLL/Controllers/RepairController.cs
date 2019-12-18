using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AgencyBanking_BLL.util;
using AgencyBanking_DAL;
using Dapper;
using Newtonsoft.Json;

namespace AgencyBanking_BLL.Controllers
{
    [Authorize]
    [CorporateNumberFilter]
  public  class RepairController: Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //GET: Repair/
        public ActionResult Index()
        {
            DeviceRepairModel model = new DeviceRepairModel();
            model.Saccoid = CurrentSacco.CorporateNo;
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
                        return View();
                    }
                    ModelState.AddModelError("Imei", "The Imei was not found");

                    return View(model);

                }
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
          
            return View(model);
        }

        private bool CheckImeiExists(string imei)
        {
            DynamicParameters par =  new DynamicParameters();
            par.Add("imei",imei);
            DeviceInfo device = DapperOrm.QueryGetSingle<DeviceInfo>("select top 1 * from deviceinfo where imei = @imei", par);
            return device != null && !string.IsNullOrEmpty(device.IMEI);
        }
        /// <summary>
        /// Given a device Imei return all the status of the device  and repair notes
        /// </summary>
        /// <param name="imei">Device Imei</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Track(string imei)
        {
            if (imei != null)
            {
             RepairBLL bll = new RepairBLL();
             string data = bll.GetDeviceStatusByIMEI(imei);
             return  Content(data, "application/json");

            }
            return new HttpNotFoundResult();
        }
        //GET /Repair/track
        public ActionResult Track()
        {
            RepairBLL bll = new RepairBLL();
            IEnumerable<string> data = bll.GetALLDevices_UnderRepair();
            if (data != null) ViewBag.data = data;
            return View();
        }
    }
}
