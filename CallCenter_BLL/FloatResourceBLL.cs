using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_DAL;

namespace CallCenter_BLL
{
  public class FloatResourceBLL
  {
#if DEBUG
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
#else
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
#endif
    
    string _query;
    string _tblFloatResource = FloatResource.DBTableName;
    public IEnumerable<FloatResource> GetFloatResourcesList()
    {
      _query = $@"SELECT * FROM {_tblFloatResource}";
      return new DapperORM(_connString).QueryGetList<FloatResource>(_query);
    }

    public FloatResource GetFloatResourceByName(string resourceName)
    {
      _query = $@"SELECT * FROM {_tblFloatResource} WHERE ResourceName='{resourceName}'";
      return new DapperORM(_connString).QueryGetSingle<FloatResource>(_query);
    }
    public FloatResource GetFloatResourceById(string resourceId)
    {
      _query = $@"SELECT * FROM {_tblFloatResource} WHERE Id='{resourceId}'";
      return new DapperORM(_connString).QueryGetSingle<FloatResource>(_query);
    }
  }
}
