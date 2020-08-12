using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.ViewModels
{
  public class MSACCOPlusRegisteredMemberDeviceViewModel: IMsaccoPlusNumberCheckerViewModel
  {
    public int Id { get; set; }

    public string PhoneNumber { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime DateLinked { get; set; }

    public string Comments { get; set; }
  }
}
