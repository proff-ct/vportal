using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
   public class DeviceModel
    {
        public String id{ get; set; }
        [Required(ErrorMessage = "The Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Imei Is Required as it Uniquely identify the Device")]
        public string IMEI { get; set; }
        
        public bool Enabled { get; set; }

        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public bool Assigned { get; set; }
        public string Organization { get; set; }
        public string Region { get; set; }

    }
}
