using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
   public class DeviceRepairModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The Device Imei is Required")]
        public string Imei { get; set; }
        [Required(ErrorMessage = "The Problem Description is Required")]
        public string Description { get; set; }

        public  string Saccoid { get; set; }
    }
}
