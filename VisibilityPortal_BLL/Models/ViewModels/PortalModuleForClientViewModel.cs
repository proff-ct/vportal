using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_BLL.Models.ViewModels
{
  public class PortalModuleForClientViewModel
  {
    public string ClientModuleId { get; set; }
    public string ClientCorporateNo { get; set; }
    public string PortalModuleName { get; set; }
    public bool IsEnabled { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; private set; }
  }
}
