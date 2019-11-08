using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
  public  class DeviceAssignModel
    {

        [Required(ErrorMessage = "The Agent Code is is invalid")]
        public String AgentCode { get; set; }

        [Required(ErrorMessage = "The Device Imei doesnt Exist Consider Registering it here <a href='/device'>Register device</a>")]
        public String DeviceImei { get; set; }
    }
}
