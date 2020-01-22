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
  public class MSaccoAirtimeTopupBLL
  {
    private string _query;
    private readonly string _tblMSaccoAirtimeTopup = MSaccoAirtimeTopup.DBTableName;
    public IEnumerable<MSaccoAirtimeTopup> GetMSaccoAirtimeTopupTrxListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} 
          WHERE [Corporate No]='{corporateNo}'
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoAirtimeTopup}
          WHERE [Corporate No]='{corporateNo}'
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<MSaccoAirtimeTopup> loans = results.Read<MSaccoAirtimeTopup>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return loans;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} WHERE [Corporate No]='{corporateNo}' ORDER BY [Entry No] DESC";
        return new DapperORM().QueryGetList<MSaccoAirtimeTopup>(_query);
      }

    }
  public IEnumerable<MSaccoAirtimeTopup> GetClientMSaccoAirtimeTopupTrxListForToday(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} 
          WHERE [Corporate No]='{corporateNo}'
          AND datediff(dd, [Transaction Date], getdate()) = 0
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoAirtimeTopup}
          WHERE [Corporate No]='{corporateNo}'
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<MSaccoAirtimeTopup> loans = results.Read<MSaccoAirtimeTopup>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return loans;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} 
                  WHERE [Corporate No]='{corporateNo}' 
                  AND datediff(dd, [Transaction Date], getdate()) = 0";
        return new DapperORM().QueryGetList<MSaccoAirtimeTopup>(_query);
      }

    }
  }
}
