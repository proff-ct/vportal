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
    private readonly string _tblMSaccoAirtimeTopup = MSaccoAirtimeTopup.DBTableName;
    public IEnumerable<MSaccoAirtimeTopup> GetMSaccoAirtimeTopupTrxListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} 
          WHERE [Corporate No]=@CorporateNo
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoAirtimeTopup}
          WHERE [Corporate No]=@CorporateNo
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
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
        query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} WHERE [Corporate No]=@CorporateNo ORDER BY [Entry No] DESC";
        return new DapperORM().QueryGetList<MSaccoAirtimeTopup>(query, qryParams);
      }

    }
  public IEnumerable<MSaccoAirtimeTopup> GetClientMSaccoAirtimeTopupTrxListForToday(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} 
          WHERE [Corporate No]=@CorporateNo
          AND datediff(dd, [Transaction Date], getdate()) = 0
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMSaccoAirtimeTopup}
          WHERE [Corporate No]=@CorporateNo
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
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
        query = $@"SELECT * FROM {_tblMSaccoAirtimeTopup} 
                  WHERE [Corporate No]=@CorporateNo 
                  AND datediff(dd, [Transaction Date], getdate()) = 0";
        return new DapperORM().QueryGetList<MSaccoAirtimeTopup>(query, qryParams);
      }

    }
  }
}
