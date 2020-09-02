using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_Dataspecs.Models
{
  public interface IUserRole
  {
    string AspRoleId { get; set; }
    string AspRoleName { get; set; }
    string ClientModuleId { get; set; }
    bool IsEnabled { get; set; }
  }
  public static class PortalUserRoles
  {
    // stored in db in AspNetRoles table 
    // these are the default "system" roles
    public enum SystemRoles
    {
      SuperAdmin,
      SystemAdmin,
      Admin,
      Regular
    }
  }
}
