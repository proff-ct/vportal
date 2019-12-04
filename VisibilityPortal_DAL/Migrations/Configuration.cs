namespace VisibilityPortal_DAL.Migrations
{
  using System;
  using System.Collections.Generic;
  using System.Data.Entity.Migrations;

  internal sealed class Configuration : DbMigrationsConfiguration<PortalDBContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(PortalDBContext context)
    {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data.
      // register the portal modules
      List<PortalModule> portalModules = new List<PortalModule>
      {
        new PortalModule
        {
          ModuleName = PortalModule.modules.AgencyBanking.ToString(),
          CoreTecProductName = "Agency Banking",
          RoutePrefix = PortalModule.AgencyBankingModule.routePrefix
        },
        new PortalModule
        {
          ModuleName = PortalModule.modules.MSacco.ToString(),
          CoreTecProductName = "MSacco",
          RoutePrefix = PortalModule.MSaccoModule.routePrefix
        },
        new PortalModule
        {
          ModuleName = PortalModule.modules.CallCenter.ToString(),
          CoreTecProductName = PortalModule.PlaceholderIfNotCoretecProduct,
          RoutePrefix = PortalModule.CallCenterModule.routePrefix
        }
    };
      context.PortalModules.AddOrUpdate(m => m.ModuleName, portalModules.ToArray());

      // register the Coretec staff facing modules
      List<PortalModuleForClient> coretecStaffOnlyModules = new List<PortalModuleForClient>();
      portalModules
        .FindAll(m => m.CoreTecProductName == PortalModule.PlaceholderIfNotCoretecProduct)
        .ForEach(m => {
          coretecStaffOnlyModules.Add(new PortalModuleForClient
          {
            ClientModuleId = Guid.NewGuid().ToString(),
            ClientCorporateNo = "CORETEC",
            PortalModuleName = m.ModuleName,
            CreatedBy = "PORTAL SETUP",
            IsEnabled = true
          });
        });
      context.PortalModuleForClients.AddOrUpdate(m => m.PortalModuleName, coretecStaffOnlyModules.ToArray());
    }
  }
}
