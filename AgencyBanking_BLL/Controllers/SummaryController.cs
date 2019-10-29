using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AgencyBanking_BLL.Controllers
{
   public class SummaryController: Controller
    {
        [HttpGet]
        public ActionResult Index(string type)
        {
            return Content(type);
        }

        public ActionResult Devices()
        {
            var deviceinfo = new DeviceInfoBLL();
            ViewBag.caption = "Registered Devices";
            ViewBag.link = "devices";
            ViewBag.Title = "Registered Devices";
            ViewBag.columns = DeviceinfoHeader();
            var devices = deviceinfo.GetDevicesByOrganization("mama");
            ViewBag.data = JsonConvert.SerializeObject(devices);
            return View("List");
        }

        public string DeviceinfoHeader()
        {
            List<TableHeader> list = new List<TableHeader>();
          
            list.Add(new TableHeader
            {
                field = "IMEI",
                title = "IMEI",
                sorter = "number",
                headerFilter = "input"
            });
            list.Add(new TableHeader
            {
                field = "Enabled",
                title = "Enabled",
                sorter = "string"
            });
            list.Add(new TableHeader
            {
                field = "Region",
                title = "Region",
                sorter = "string",
                headerFilter = "input"
            });
            return  JsonConvert.SerializeObject(list);
        }
    }
}
