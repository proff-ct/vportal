using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_DAL;

namespace CallCenter_BLL
{
  public class AlertTypeBLL
  {
#if DEBUG
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
#else
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
#endif
    
    string _query;
    string _tblAlertType = AlertType.DBTableName;
    public IEnumerable<AlertType> GetAllAlertTypes()
    {
      _query = $@"SELECT * FROM {_tblAlertType}";
      return new DapperORM(_connString).QueryGetList<AlertType>(_query);
    }

    public AlertType GetAlertTypeByName(string alertTypeName)
    {
      _query = $@"SELECT * FROM {_tblAlertType} WHERE AlertName='{alertTypeName}'";
      return new DapperORM(_connString).QueryGetSingle<AlertType>(_query);
    }
  }
}
