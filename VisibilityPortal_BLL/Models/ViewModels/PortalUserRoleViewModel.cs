using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL.Models.ViewModels
{
  public class PortalUserRoleViewModel
  {
    public string UserId { get; set; }
    public string ClientModuleId { get; set; }
    public string Module { get; set; }
    public string AspRoleId { get; set; }
    public string AspRoleName { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; private set; }
    public bool IsEnabled { get; set; }
  }
}
