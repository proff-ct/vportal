using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL.Models
{
  public class CoretecClientWithModule : CoreTecClient
  {
    public virtual IEnumerable<PortalModuleForClient> Modules { get; set; }
  }
}
