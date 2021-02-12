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
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Models;

namespace MSacco_BLL
{
  public class MsaccoBankTransferBLL : IBL_BankTransfer
  {
    private string _query;
    private IBL_SACCO _saccoBLL;
    private readonly string _tblMsaccoBankTransfer = MSACCOBankTransfer.DBTableName;
    private readonly string _pesalinkDBCStr = @ConfigurationManager.ConnectionStrings["pesaLinkDB_prod"].ConnectionString;

    public MsaccoBankTransferBLL(IBL_SACCO saccoBLL)
    {
      _saccoBLL = saccoBLL ?? throw new ArgumentNullException("NULL SACCO BLL given to MsaccoBankTransferBLL!");
    }
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

    public bool IsClientRegisteredForBankTransfer(string corporateNo)
    {
      string tblPesaLinkCharges = "CorporateCharges";

      _query = $@"SELECT CASE WHEN EXISTS (
                    SELECT *
                    FROM {tblPesaLinkCharges}
                    WHERE [CorporateNo] = '{corporateNo}'
                  )
                  THEN CAST(1 AS BIT)
                  ELSE CAST(0 AS BIT)
                  END";

      return new DapperORM(_pesalinkDBCStr).QueryGetSingle<bool>(_query);
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

    public bool IsClientUsingCoretecFloat(string corporateNo)
    {
      
      ISACCO sacco = _saccoBLL.GetSaccoByUniqueParam(corporateNo);
      if (sacco != null)
      {
        _query = $@"SELECT [UseCoretecPesalinkFloat]
              FROM {sacco.tableName}
              WHERE [Corporate No]='{corporateNo}'";
        return new DapperORM().QueryGetSingle<bool>(_query);
      }
      else throw new ArgumentException($"No SACCO found matching: {corporateNo}");      
    }

    public IClientBankTransferFloat GetClientFloat(string corporateNo)
    {
      ISACCO sacco = _saccoBLL.GetSaccoByUniqueParam(corporateNo);
      if (sacco != null)
      {
        _query = $@"SELECT *
              FROM {PesalinkFloatBalance.DBTableName}
              WHERE [CorporateNo]='{corporateNo}'";

        IPesalinkFloatBalance pesaLinkFloat = new DapperORM(_pesalinkDBCStr).QueryGetSingle<PesalinkFloatBalance>(_query);

        try
        {
          return new ClientFloat(pesaLinkFloat.Amount, pesaLinkFloat.Last_Updated);
        }
        catch(NullReferenceException ex)
        {
          throw new ArgumentNullException($"No float balance record(s) found for {corporateNo}");
        }
      }
      else throw new ArgumentException($"No client record found matching: {corporateNo}");
    }

    public class ClientFloat : IClientBankTransferFloat
    {
      public ClientFloat(decimal balance, DateTime lastTransactionTimeStamp)
      {
        CurrentFloat = balance;
        FloatTransactionTimeStamp = lastTransactionTimeStamp;
      }
      public decimal CurrentFloat { get; }

      public DateTime FloatTransactionTimeStamp { get; }
    }

    
  }
}
