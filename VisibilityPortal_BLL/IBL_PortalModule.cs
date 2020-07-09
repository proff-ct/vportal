using System.Collections.Generic;
using VisibilityPortal_DAL;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL
{
  public interface IBL_PortalModule
  {
    string GetDefaultRouteForModule(string moduleName);
    //PortalModule GetModuleByName(string moduleName);
    //IEnumerable<PortalModule> GetModulesList();
    IPortalModule GetModuleByName(string moduleName);
    IEnumerable<IPortalModule> GetModulesList();
  }
}