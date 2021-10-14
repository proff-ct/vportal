using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSacco_Dataspecs.Feature.MsaccoWhitelisting.Models;

namespace MSacco_BLL.Models.ViewModels
{
  public class MSACCORegistrationToWhitelist : IRegistrationRecordToWhitelistViewModel
  {
    public string PhoneNumber { get; set; }
    public DateTime DateRegistered { get; set; }
  }
}
