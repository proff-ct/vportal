using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using VisibilityPortal_BLL.InitialSetup.SetupDefaults;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;

namespace VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig
{
  public class PortalDatabaseInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
  {
    protected override void Seed(ApplicationDbContext context)
    {
      RoleStore<ApplicationRole> roleStore = new RoleStore<ApplicationRole>(context);
      RoleManager<ApplicationRole> roleManager = new RoleManager<ApplicationRole>(roleStore);

      List<ApplicationRole>applicationRoles = new List<ApplicationRole>
      {
        new ApplicationRole()
        {
          Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey= ApplicationRoleDefaults.PASSKEY_DEFAULT
        },
        new ApplicationRole()
        {
          Name = PortalUserRoles.SystemRoles.SystemAdmin.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey= ApplicationRoleDefaults.PASSKEY_DEFAULT
        },
        new ApplicationRole()
        {
          Name = PortalUserRoles.SystemRoles.Admin.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey= ApplicationRoleDefaults.PASSKEY_DEFAULT
        },
        new ApplicationRole()
        {
          Name = PortalUserRoles.SystemRoles.Regular.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey= ApplicationRoleDefaults.PASSKEY_DEFAULT
        }
      };

      foreach (ApplicationRole role in applicationRoles)
      {
        roleManager.Create(role);
      }

      base.Seed(context);
    }
  }
}
