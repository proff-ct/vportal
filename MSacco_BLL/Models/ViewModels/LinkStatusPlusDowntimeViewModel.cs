using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.ViewModels
{
  public class LinkStatusPlusDowntimeViewModel : LinkMonitoringViewModel
  {
    public virtual IEnumerable<LinkDowntimeViewModel> Downtimes { get; set; }
  }
}
