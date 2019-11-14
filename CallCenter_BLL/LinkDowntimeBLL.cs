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
  public class LinkDowntimeBLL
  {
    private string _query;
    private readonly string _tblLinkDowntime = LinkDowntime.DBTableName;
    public IEnumerable<LinkDowntime> GetDowntimeRecordsForClient(string corporateNo)
    {
      _query = $@"SELECT * FROM {_tblLinkDowntime}
                  WHERE [Corporate No]='{corporateNo}'
                  ORDER BY[Entry No] DESC";

      return new DapperORM().QueryGetList<LinkDowntime>(_query);
    }

    public IEnumerable<LinkDowntime> GetDowntimeRecordsForAllClients(
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblLinkDowntime} 
          ORDER BY [Corporate Name] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Corporate Name]) as TotalRecords  
          FROM {_tblLinkDowntime}
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<LinkDowntime> records = results.Read<LinkDowntime>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblLinkDowntime}";
        return new DapperORM().QueryGetList<LinkDowntime>(_query);
      }

    }
  }
}
