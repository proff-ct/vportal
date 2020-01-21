using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.ViewModels
{
  public class LinkDowntimeViewModel 
  {
    public int Entry_No { get; set; }
    public DateTime? Downtime_Start { get; set; }
    public DateTime? Downtime_Stop { get; set; }
    public string Corporate_No { get; set; }
    public string Comment { get; set; }
    public string Url { get; set; }
    public DateTime? Last_Check { get; set; }
    public string Corporate_Name { get; set; }
    public string Downtime_Duration { get; set; }
  }
}
