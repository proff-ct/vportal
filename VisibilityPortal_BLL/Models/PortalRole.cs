using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL.Models
{
  public class PortalRole : IUserRole
  {
    public string AspRoleId { get; set; }
    public string AspRoleName { get; set; }
    public string ClientModuleId { get; set; }
    public bool IsEnabled { get; set; }
    
    public PortalRole()
    {

    }
    public PortalRole(string clientModuleId, string aspRoleId, string aspRoleName)
    {
      ClientModuleId = clientModuleId;
      AspRoleId = aspRoleId;
      AspRoleName = aspRoleName;
    }
  }
}
