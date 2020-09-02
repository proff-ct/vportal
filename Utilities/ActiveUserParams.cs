using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_Dataspecs.Models;

namespace Utilities.PortalApplicationParams
{
  public class ActiveUserParams : IActiveUserParams
  {
    public string ClientCorporateNo { get; set; }
    public string ClientModuleId { get; set; }
    public List<UserRoles> Roles { get; set; }
    public class UserRoles : IUserRole
    {
      public string ClientModuleId { get; set; }
      public string AspRoleName { get; set; }
      public string AspRoleId { get; set; }
      public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// The Named Session Variable for this class
    /// Can be called from anywhere in the code.
    /// </summary>
    public static string SessionVaribleName()
    {
     return "ActiveUserParams";
    } 
  }
}
