namespace VisibilityPortal_BLL.Migrations
{
  using System.Data.Entity.Migrations;
  using VisibilityPortal_BLL.InitialSetup.SetupDefaults;
  using VisibilityPortal_BLL.Models;
  using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;

  internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
      ContextKey = "VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels.ApplicationDbContext";
    }

    protected override void Seed(ApplicationDbContext context)
    {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data. E.g.
      //
      //    context.People.AddOrUpdate(
      //      p => p.FullName,
      //      new Person { FullName = "Andrew Peters" },
      //      new Person { FullName = "Brice Lambson" },
      //      new Person { FullName = "Rowan Miller" }
      //    );
      //

      context.Roles.AddOrUpdate(
        r => r.Name,
        new ApplicationRole
        {
          Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey = ApplicationRoleDefaults.PASSKEY_DEFAULT
        },
        new ApplicationRole
        {
          Name = PortalUserRoles.SystemRoles.SystemAdmin.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey = ApplicationRoleDefaults.PASSKEY_DEFAULT
        },
        new ApplicationRole
        {
          Name = PortalUserRoles.SystemRoles.Admin.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey = ApplicationRoleDefaults.PASSKEY_DEFAULT
        },
        new ApplicationRole
        {
          Name = PortalUserRoles.SystemRoles.Regular.ToString(),
          SetupPassPhrase = ApplicationRoleDefaults.PASSPHRASE_DEFAULT,
          SetupKey = ApplicationRoleDefaults.PASSKEY_DEFAULT
        }
        );

    }
  }
}
