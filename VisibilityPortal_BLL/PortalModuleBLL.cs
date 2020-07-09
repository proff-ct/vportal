using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_DAL;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL
{
  public class PortalModuleBLL : IBL_PortalModule
  {
#if DEBUG
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
#else
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
#endif
    
    string _query;
    string _tblPortalModule = PortalModule.DBTableName;
    public IEnumerable<IPortalModule> GetModulesList()
    {
      _query = $@"SELECT * FROM {_tblPortalModule}";
      return new DapperORM(_connString).QueryGetList<PortalModule>(_query);
    }

    public IPortalModule GetModuleByName(string moduleName)
    {
      _query = $@"SELECT * FROM {_tblPortalModule} WHERE ModuleName='{moduleName}'";
      return new DapperORM(_connString).QueryGetSingle<PortalModule>(_query);
    }

    public string GetDefaultRouteForModule(string moduleName)
    {
      
      if (moduleName == PortalModule.AgencyBankingModule.moduleName)
      {
        return PortalModule.AgencyBankingModule.defaultRoute;
      }
      else if (moduleName == PortalModule.MSaccoModule.moduleName)
      {
        return PortalModule.MSaccoModule.defaultRoute;
      }
      else if (moduleName == PortalModule.CallCenterModule.moduleName)
      {
        return PortalModule.MSaccoModule.defaultRoute;
      }
      else
      {
        if (string.IsNullOrEmpty(moduleName))
        {
          throw new ArgumentNullException(
            message:"No module supplied!", paramName: nameof(moduleName));
        }
        else
        {
          throw new ArgumentException(
            message: $"No configuration found for module: {moduleName}", paramName: nameof(moduleName));
        }
        
      }
    }
  }
}
