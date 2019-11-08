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
  public class MobileWithdrawalsBLL
  {
    private string _query;
    private readonly string _tblMobileWithdrawals = MobileWithdrawals.DBTableName;
    public IEnumerable<MobileWithdrawals> GetMobileWithdrawalsTrxListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMobileWithdrawals} 
          WHERE [Corporate No]='{corporateNo}'
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMobileWithdrawals}
          WHERE [Corporate No]='{corporateNo}'
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(DapperORM.ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<MobileWithdrawals> records = results.Read<MobileWithdrawals>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblMobileWithdrawals} WHERE [Corporate No]='{corporateNo}'";
        return DapperORM.QueryGetList<MobileWithdrawals>(_query);
      }

    }
  }
}
