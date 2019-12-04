namespace VisibilityPortal_DAL.Migrations
{
  using System;
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
      context.PortalModules.AddOrUpdate(
        m => m.ModuleName,
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
        });

      // register the Coretec client modules
      context.PortalModuleForClients.AddOrUpdate(new PortalModuleForClient
      {
        ClientModuleId = Guid.NewGuid().ToString(),
        ClientCorporateNo = "CORETEC",
        PortalModuleName = PortalModule.modules.CallCenter.ToString(),
        CreatedBy = "PORTAL SETUP",
        IsEnabled = true
      });
    }
  }
}
