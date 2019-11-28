using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace VisibilityPortal_DAL.EF6_related
{
  public class PortalDatabaseInitializer_DAL : DropCreateDatabaseIfModelChanges<PortalDBContext>
  {
    protected override void Seed(PortalDBContext context)
    {
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
          CoreTecProductName = "N/A",
          RoutePrefix = PortalModule.CallCenterModule.routePrefix
        }
      };
      List<PortalModule> coretecPortalModules = portalModules
        .FindAll(m => m.CoreTecProductName == "N/A");
      List<PortalModuleForClient> coretecStaffOnlyModules = new List<PortalModuleForClient>();
      coretecPortalModules.ForEach(m => {
        coretecStaffOnlyModules.Add(new PortalModuleForClient
        {
          ClientModuleId = Guid.NewGuid().ToString(),
          ClientCorporateNo = "CORETEC",
          PortalModuleName = m.ModuleName,
          CreatedBy = "PORTAL SETUP",
          IsEnabled = true
        });
      });
      context.PortalModules.AddOrUpdate(m=>m.ModuleName, portalModules.ToArray());
      context.PortalModuleForClients.AddOrUpdate(
        m => m.PortalModuleName, coretecStaffOnlyModules.ToArray());

      base.Seed(context);
    }
  }
}
