using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_BLL.Models.ViewModels
{
  public class AddPortalModuleForClientViewModel
  {
    public string CorporateNo { get; set; }
    public IList<string> PortalModules { get; set; }
  }
}
