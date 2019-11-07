using System.Collections.Generic;
using CallCenter_DAL;

namespace CallCenter_BLL
{
  public class LinkMonitoringBLL
  {
    private string _query;
    private readonly string _tblLinkMonitoring = LinkMonitoring.DBTableName;
    public LinkMonitoring GetLinkInfoForClient(string corporateNo)
    {
      _query = $@"SELECT * FROM {_tblLinkMonitoring} WHERE [Corporate No]='{corporateNo}'";
      return DapperORM.QueryGetSingle<LinkMonitoring>(_query);
    }

    public IEnumerable<LinkMonitoring> GetLinkInfoForAllClients()
    {
      _query = $@"SELECT * FROM {_tblLinkMonitoring}";
      return DapperORM.QueryGetList<LinkMonitoring>(_query);
    }
  }
}
