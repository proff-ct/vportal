using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace CallCenter_DAL.EF6_related
{
  public class CallCenterDatabaseInitializer_DAL : DropCreateDatabaseIfModelChanges<CallCenterDBContext>
  {
    protected override void Seed(CallCenterDBContext context)
    {
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
       
      context.AlertTypes.AddOrUpdate(a=>a.AlertName, alertTypes.ToArray());
      context.FloatResources.AddOrUpdate(r=>r.ResourceName, floatResources.ToArray());
      base.Seed(context);
    }
  }
}
