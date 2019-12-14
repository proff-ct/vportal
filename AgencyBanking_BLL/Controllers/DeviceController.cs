using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AgencyBanking_BLL.util;
using AgencyBanking_DAL;
using Dapper;

namespace AgencyBanking_BLL.Controllers
{
    [Authorize]
 public class DeviceController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Create a new Device";
            return View("create");
        }

        //post: device/create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeviceModel model)
        {
            if (!ModelState.IsValid)
            {
                return Content(new StatusMessage()
                {
                    Code = "500",
                    Message = "Some Information are Missing Kindly<br> Kindly all the fields"
                }.toJson());
            }
            return Content(new DeviceInfoBLL().InsertNewDevice(model));
        }
        /// <summary>
        /// Assigns a particular device to  a particular agent given the imei and the agent code
        /// </summary>
        /// <returns></returns>
       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(DeviceAssignModel model)
        {
            if (ModelState.IsValid)
            {
                DeviceInfoBLL devicebll = new DeviceInfoBLL();

                if (devicebll.AssignDevice(model))
                {
                    return Content(new StatusMessage()
                    {
                        Code = "Success",
                        Message = "Device was added successfully"
                    }.toJson());
                }
            }

            ModelState.AddModelError("AgentCode","The agent code is either invalid");
            ModelState.AddModelError("DeviceImei", "The Device IMEI doesnt Exist Consider Registering it here");
            return Content(new StatusMessage()
            {
                Code="500",
                Message = "The agent code or Device Imei  is invalid"
            }.toJson());

        }

        public ActionResult Assign()
        {
            ViewBag.Title = "Assign a Device to an Agent";
            return View("Assign");
        }
    }
}
