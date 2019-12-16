using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSacco_DAL;

namespace MSacco_BLL.Models
{
  public class LinkStatusPlusDowntime : LinkMonitoring
  {
    public virtual IEnumerable<LinkDowntime> Downtimes { get; set; }
  }
}
