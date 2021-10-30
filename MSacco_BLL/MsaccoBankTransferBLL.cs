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
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMsaccoBankTransfer} 
          WHERE [Corporate No]=@CorporateNo
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoBankTransfer}
          WHERE [Corporate No]=@CorporateNo
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
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
        query = $@"SELECT * FROM {_tblMsaccoBankTransfer} WHERE [Corporate No] = @CorporateNo ";
        return new DapperORM().QueryGetList<MSACCOBankTransfer>(query, qryParams);
      }

    }

    public bool IsClientRegisteredForBankTransfer(string corporateNo)
    {
#if DEBUG
      return false;
#endif
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string tblPesaLinkCharges = "CorporateCharges";

      string query = $@"SELECT CASE WHEN EXISTS (
                    SELECT *
                    FROM {tblPesaLinkCharges}
                    WHERE [CorporateNo] = @CorporateNo
                  )
                  THEN CAST(1 AS BIT)
                  ELSE CAST(0 AS BIT)
                  END";

      return new DapperORM(_pesalinkDBCStr).QueryGetSingle<bool>(query, qryParams);
    }

    public IEnumerable<IBankTransfer> GetClientBankTransferRecordsForToday(
      string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
    {
      lastPage = 0;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMsaccoBankTransfer} 
          WHERE [Corporate No]=@CorporateNo
          AND datediff(dd, [TransactionDate], getdate()) = 0
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoBankTransfer}
          WHERE [Corporate No]=@CorporateNo
          AND datediff(dd, [TransactionDate], getdate()) = 0
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
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
        query = $@"SELECT * FROM {_tblMsaccoBankTransfer} 
                  WHERE [Corporate No]=@CorporateNo AND datediff(dd, [TransactionDate], getdate()) = 0";
        return new DapperORM().QueryGetList<MSACCOBankTransfer>(query, qryParams);
      }

    }

    public bool IsClientUsingCoretecFloat(string corporateNo)
    {
#if DEBUG
      return false;
#endif
      ISACCO sacco = _saccoBLL.GetSaccoByUniqueParam(corporateNo);
      if (sacco != null)
      {
        DynamicParameters qryParams = new DynamicParameters();
        qryParams.Add("CorporateNo", corporateNo);
        string query = $@"SELECT [UseCoretecPesalinkFloat]
              FROM {Sacco.DBTableName}
              WHERE [Corporate No]=@CorporateNo";
        return new DapperORM().QueryGetSingle<bool>(query, qryParams);
      }
      else throw new ArgumentException($"No SACCO found matching: {corporateNo}");      
    }

    public IClientBankTransferFloat GetClientFloat(string corporateNo)
    {
      ISACCO sacco = _saccoBLL.GetSaccoByUniqueParam(corporateNo);
      if (sacco != null)
      {
        DynamicParameters qryParams = new DynamicParameters();
        qryParams.Add("CorporateNo", corporateNo);
        string query = $@"SELECT *
              FROM {PesalinkFloatBalance.DBTableName}
              WHERE [CorporateNo]=@CorporateNo";

        IPesalinkFloatBalance pesaLinkFloat = new DapperORM(_pesalinkDBCStr).QueryGetSingle<PesalinkFloatBalance>(query, qryParams);

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
