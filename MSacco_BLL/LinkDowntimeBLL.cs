using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using Dapper;

namespace MSacco_BLL
{
  public class LinkDowntimeBLL
  {
    private readonly string _tblLinkDowntime = LinkDowntime.DBTableName;
    public IEnumerable<LinkDowntime> GetDowntimeRecordsForClient(string corporateNo)
    {
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);

      string query = $@"SELECT * FROM {_tblLinkDowntime}
                  WHERE [Corporate No]=@CorporateNo
                  ORDER BY[Entry No] DESC";

      return new DapperORM().QueryGetList<LinkDowntime>(query, qryParams);
    }

    public IEnumerable<LinkDowntime> GetDowntimeRecordsForAllClients(
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      string query;
      if (paginate)
      {
        query = $@"SELECT * FROM {_tblLinkDowntime} 
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
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, dp, commandType: CommandType.Text))
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
        query = $@"SELECT * FROM {_tblLinkDowntime}";
        return new DapperORM().QueryGetList<LinkDowntime>(query);
      }

    }
  }
}
