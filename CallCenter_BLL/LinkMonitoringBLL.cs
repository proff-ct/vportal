using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_DAL;
using Dapper;

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

    public IEnumerable<LinkMonitoring> GetLinkInfoForAllClients(
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblLinkMonitoring} 
          ORDER BY [Corporate Name] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Corporate Name]) as TotalRecords  
          FROM {_tblLinkMonitoring}
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(DapperORM.ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<LinkMonitoring> records = results.Read<LinkMonitoring>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblLinkMonitoring}";
        return DapperORM.QueryGetList<LinkMonitoring>(_query);
      }

    }
  }
}
