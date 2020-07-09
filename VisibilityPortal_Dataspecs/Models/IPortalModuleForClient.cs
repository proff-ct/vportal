using System;

namespace VisibilityPortal_Dataspecs.Models
{
  public interface IPortalModuleForClient
  {
    string ClientCorporateNo { get; set; }
    string ClientModuleId { get; set; }
    string CreatedBy { get; set; }
    DateTime CreatedOn { get; }
    bool IsEnabled { get; set; }
    string ModifiedBy { get; set; }
    DateTime ModifiedOn { get; }
    string PortalModuleName { get; set; }
  }
}