using System.Collections.Generic;

namespace Utilities.PortalApplicationParams
{
  public interface IActiveUserParams
  {
    string ClientCorporateNo { get; set; }
    string ClientModuleId { get; set; }
    List<ActiveUserParams.UserRoles> Roles { get; set; }
    string APIAuthID { get; set; }
    string APIToken { get; }
  }
}