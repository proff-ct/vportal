namespace VisibilityPortal_DAL.Migrations
{
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
          CoreTecProductName = "N/A",
          RoutePrefix = PortalModule.CallCenterModule.routePrefix
        });

    }
  }
}
