using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AgencyBanking_DAL;

namespace AgencyBanking_BLL.Controllers
{
   public class SummaryController: Controller
    {
        [HttpGet]
        public ActionResult Index(string type)
        {
            var summary  = new SummaryBLL();
            var data = summary.GetSummaryByName(type);
            ViewBag.caption = $"Summary of {type}";
            ViewBag.link = "";
            ViewBag.columns = SetHeaders(new TransactionModel());
           ViewBag.Title = ViewBag.caption;
            ViewBag.data = JsonConvert.SerializeObject(data);
            return View("list");

        }
        [HttpGet]
        public ActionResult Agents(string BankNo)
        {
            var agents = new AgentsBLL();
            ViewBag.caption = "Agents Belonging to";
            ViewBag.link = "Agents";
            ViewBag.Title = "Agents";
            ViewBag.columns = SetHeaders(new AgentModel());
            var devices = agents.GetAgentsByOrganization();
            ViewBag.data = JsonConvert.SerializeObject(devices);
            return View("List");
        }
        public ActionResult Devices()
        {
            var deviceinfo = new DeviceInfoBLL();
           
            ViewBag.caption = "Registered Devices Belonging to";
            ViewBag.link = "devices";
            ViewBag.Title = "Registered Devices Belonging to";
            ViewBag.columns = DeviceinfoHeader();
            var devices = deviceinfo.GetDevicesByOrganization("mama");
            ViewBag.data = JsonConvert.SerializeObject(devices);
            return View("list");
        }

        private string SetHeaders<T>(T t)
        {
            var Properties = GetProperties(t);
            var list = new List<TableHeader>();
            foreach (var pro in Properties)
            {
               list.Add(new TableHeader()
               {
                   field = pro.Name,
                   title = pro.Name,
                   sorter = "string",
                   headerFilter = "input"
               }); 
            }

            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// Using Reflection to Get Properties
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }

        private string DeviceinfoHeader()
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
