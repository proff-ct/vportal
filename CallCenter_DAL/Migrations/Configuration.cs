namespace CallCenter_DAL.Migrations
{
  using System;
  using System.Collections.Generic;
  using System.Data.Entity.Migrations;
  using System.Linq;

  internal sealed class Configuration : DbMigrationsConfiguration<CallCenterDBContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(CallCenterDBContext context)
    {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data.
      List<AlertType> alertTypes = new List<AlertType>();
      List<FloatResource> floatResources = new List<FloatResource>();

      foreach (string alertType in Enum.GetNames(typeof(AlertType.AlertTypes)))
      {
        alertTypes.Add(new AlertType
        {
          AlertName = alertType,
          CreatedBy = "PORTAL SETUP"
        });
      }
      foreach (string resource in Enum.GetNames(typeof(FloatResource.FloatResources)))
      {
        floatResources.Add(new FloatResource
        {
          Id = Guid.NewGuid().ToString(),
          ResourceName = resource,
          CreatedBy = "PORTAL SETUP"
        });
      }

      context.AlertTypes.AddOrUpdate(a => a.AlertName, alertTypes.ToArray());
      context.FloatResources.AddOrUpdate(r => r.ResourceName, floatResources.ToArray());
    }
  }
}
