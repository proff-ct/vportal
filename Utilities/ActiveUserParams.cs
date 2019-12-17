﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.PortalApplicationParams
{
  public class ActiveUserParams
  {
    public string ClientCorporateNo { get; set; }
    public string ClientModuleId { get; set; }
    public List<UserRoles> Roles { get; set; }
    public class UserRoles
    {
      public string AspRoleId { get; set; }
      public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// The Named Session Variable for this class
    /// Can be called from anywhere in the code.
    /// </summary>
    public static string SessionVaribleName => "ActiveUserParams";
  }
}
