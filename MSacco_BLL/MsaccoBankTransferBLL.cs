using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Functions;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Configuration;

namespace MSacco_BLL
{
  public class MsaccoBankTransferBLL : IBL_BankTransfer
  {
    private string _query;
    private readonly string _tblMsaccoBankTransfer = MSACCOBankTransfer.DBTableName;
    public IEnumerable<IBankTransfer> GetBankTransferRecordsForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      IPaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMsaccoBankTransfer} 
          WHERE [Corporate No]='{corporateNo}'
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoBankTransfer}
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
            IEnumerable<IBankTransfer> records = results.Read<MSACCOBankTransfer>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblMsaccoBankTransfer} WHERE [Corporate No] = '{corporateNo}' ";
        return new DapperORM().QueryGetList<MSACCOBankTransfer>(_query);
      }

    }

    public bool IsSaccoRegisteredForBankTransfer(string corporateNo)
    {
      string dbConString = @ConfigurationManager.ConnectionStrings["pesaLinkDB_prod"].ConnectionString;
      string tblPesaLinkCharges = "CorporateCharges";

      _query = $@"SELECT CASE WHEN EXISTS (
                    SELECT *
                    FROM {tblPesaLinkCharges}
                    WHERE [CorporateNo] = '{corporateNo}'
                  )
                  THEN CAST(1 AS BIT)
                  ELSE CAST(0 AS BIT)
                  END";

      return new DapperORM(dbConString).QueryGetSingle<bool>(_query);
    }

    public IEnumerable<IBankTransfer> GetClientBankTransferRecordsForToday(
      string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMsaccoBankTransfer} 
          WHERE [Corporate No]='{corporateNo}'
          AND datediff(dd, [TransactionDate], getdate()) = 0
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoBankTransfer}
          WHERE [Corporate No]='{corporateNo}'
          AND datediff(dd, [TransactionDate], getdate()) = 0
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<IBankTransfer> records = results.Read<MSACCOBankTransfer>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblMsaccoBankTransfer} 
                  WHERE [Corporate No]='{corporateNo}' AND datediff(dd, [TransactionDate], getdate()) = 0";
        return new DapperORM().QueryGetList<MSACCOBankTransfer>(_query);
      }

    }
  }
}
