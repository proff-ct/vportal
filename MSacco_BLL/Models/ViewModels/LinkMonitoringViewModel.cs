using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.ViewModels
{
  public class LinkMonitoringViewModel
  {
    public string Corporate_No { get; set; }
    public string Corporate_Name { get; set; }
    public string Ping_Result { get; set; }
    public string Http_Status { get; set; }
    public string Overall_Link_Status { get => "Still in debate";  set => value = "Still in debate"; }
    public DateTime? Last_Check { get; set; }
    public DateTime? Last_Email_Sent { get; set; }
  }
}
