﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_BLL.Models
{
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

  public class PortalRole
  {
    public string AspRoleId { get; set; }
    public string ClientModuleId { get; set; }

    public PortalRole(string aspRoleId, string clientModuleId)
    {
      AspRoleId = aspRoleId;
      ClientModuleId = clientModuleId;
    }
  }
}
