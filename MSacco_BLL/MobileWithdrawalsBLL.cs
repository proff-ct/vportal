using AutoMapper;
using Dapper;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Models;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MSacco_BLL
{
  public class MobileWithdrawalsBLL : IBL_MobileWithdrawals
  {
    private readonly string _darajaDBConn;
    private readonly string _saccoDBConn;
    private string _connString;
    private string _tblMobileWithdrawals;

    //private IMobileWithdrawals_SACCODB _fromSACCODB;
    //private IMobileWithdrawals_DarajaDB _fromDarajaDB;

    private enum DBInUse
    {
      SACCO_DB = 1,
      DARAJA_DB = 2
    }
    public MobileWithdrawalsBLL()
    {
      _darajaDBConn = @ConfigurationManager.ConnectionStrings[MS_DBConnectionStrings.DarajaDBConnectionStringName].ConnectionString;
      _saccoDBConn = new DapperORM().ConnectionString;

      //_fromSACCODB = null;
      //_fromDarajaDB = null;
    }
    public IEnumerable<IMobileWithdrawals_SACCODB> GetMobileWithdrawalsTrxListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      IPaginationParameters pagingParams = null)
    {
      lastPage = 0;
      int saccoDBLastPage = 0;
      int darajaDBLastPage = 0;

      IEnumerable<IMobileWithdrawals_SACCODB> fromSACCODB = null;
      IEnumerable<IMobileWithdrawals_DarajaDB> fromDarajaDB = null;
      string query;

      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);

      if (paginate)
      {
        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        int darajaDBRecordCount = 0;
        int saccoDBRecordCount = 0;

        for (int i = 1; i <= 2; i++)
        {
          switch (i)
          {
            case (int)DBInUse.SACCO_DB:
              _tblMobileWithdrawals = MobileWithdrawals.DBTableName;
              _connString = _saccoDBConn;
              break;

            case (int)DBInUse.DARAJA_DB:
              _tblMobileWithdrawals = DarajaDB_MobileWithdrawals.TableName;
              _connString = _darajaDBConn;
              break;
          }

          query = $@"SELECT * FROM {_tblMobileWithdrawals} 
          WHERE [Corporate No]=@CorporateNo
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMobileWithdrawals}
          WHERE [Corporate No]=@CorporateNo
          ";

          using (SqlConnection sqlCon = new SqlConnection(_connString))
          {
            sqlCon.Open();
            using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
            {
              //IEnumerable<MobileWithdrawals> records = results.Read<MobileWithdrawals>();
              switch (i)
              {
                case (int)DBInUse.SACCO_DB:
                  fromSACCODB = results.Read<MobileWithdrawals>();
                  saccoDBRecordCount = results.Read<int>().First();
                  saccoDBLastPage = (int)Math.Ceiling(saccoDBRecordCount / (decimal)pagingParams.PageSize);
                  break;

                case (int)DBInUse.DARAJA_DB:
                  fromDarajaDB = results.Read<MobileWithdrawals_Daraja>();
                  darajaDBRecordCount = results.Read<int>().First();
                  darajaDBLastPage = (int)Math.Ceiling(darajaDBRecordCount / (decimal)pagingParams.PageSize);
                  break;
              }
            }
          }
        }

        if (saccoDBLastPage > darajaDBLastPage)
        {
          lastPage = saccoDBLastPage;
        }
        else if (darajaDBLastPage > saccoDBLastPage)
        {
          lastPage = darajaDBLastPage;
        }
        else
        {
          // both recordsets are equal
          lastPage = saccoDBLastPage;
        }
      }
      else
      {
        for (int i = 1; i <= 2; i++)
        {
          switch (i)
          {
            case (int)DBInUse.SACCO_DB:
              _tblMobileWithdrawals = MobileWithdrawals.DBTableName;
              _connString = _saccoDBConn;

              query = $@"SELECT * FROM {_tblMobileWithdrawals} WHERE [Corporate No]=@CorporateNo ORDER BY [Entry No] DESC";
              fromSACCODB = new DapperORM(_connString).QueryGetList<MobileWithdrawals>(query, qryParams);
              break;

            case (int)DBInUse.DARAJA_DB:
              _tblMobileWithdrawals = DarajaDB_MobileWithdrawals.TableName;
              _connString = _darajaDBConn;

              query = $@"SELECT * FROM {_tblMobileWithdrawals} WHERE [Corporate No]=@CorporateNo ORDER BY [Entry No] DESC";
              fromDarajaDB = new DapperORM(_connString).QueryGetList<MobileWithdrawals_Daraja>(query, qryParams);
              break;
          }
        }
      }
      // return recordset in format of old db because it is the current stable and trusted platform 10Jul2020_1711
      IEnumerable<IMobileWithdrawals_SACCODB> records = (
        fromSACCODB ?? Enumerable.Empty<IMobileWithdrawals_SACCODB>()).Concat(
        Mapper.Map<IEnumerable<IMobileWithdrawals_DarajaDB>, IEnumerable<IMobileWithdrawals_SACCODB>>(fromDarajaDB) ?? Enumerable.Empty<IMobileWithdrawals_SACCODB>());

      return records;
    }
    public IEnumerable<IMobileWithdrawals_SACCODB> GetClientMobileWithdrawalsFinancialSummaryForToday(
     string corporateNo,
     out int lastPage,
     bool paginate = false,
     IPaginationParameters pagingParams = null)
    {
      lastPage = 0;
      int saccoDBLastPage = 0;
      int darajaDBLastPage = 0;

      IEnumerable<IMobileWithdrawals_SACCODB> fromSACCODB = null;
      IEnumerable<IMobileWithdrawals_DarajaDB> fromDarajaDB = null;

      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        int darajaDBRecordCount = 0;
        int saccoDBRecordCount = 0;

        for (int i = 1; i <= 2; i++)
        {
          switch (i)
          {
            case (int)DBInUse.SACCO_DB:
              _tblMobileWithdrawals = MobileWithdrawals.DBTableName;
              _connString = _saccoDBConn;
              break;

            case (int)DBInUse.DARAJA_DB:
              _tblMobileWithdrawals = DarajaDB_MobileWithdrawals.TableName;
              _connString = _darajaDBConn;
              break;
          }

          query = $@"SELECT * FROM {_tblMobileWithdrawals} 
          WHERE [Corporate No]=@CorporateNo
          AND (
                select 
                  case 
                    when [Transaction Date] is not null
                    then datediff(dd, [Transaction Date], getdate())
                    else datediff(dd, [Datetime], getdate())
                  end) = 0
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMobileWithdrawals}
          WHERE [Corporate No]=@CorporateNo
          ";

          using (SqlConnection sqlCon = new SqlConnection(_connString))
          {
            sqlCon.Open();
            using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
            {
              switch (i)
              {
                case (int)DBInUse.SACCO_DB:
                  fromSACCODB = results.Read<MobileWithdrawals>();
                  saccoDBRecordCount = results.Read<int>().First();
                  saccoDBLastPage = (int)Math.Ceiling(saccoDBRecordCount / (decimal)pagingParams.PageSize);
                  break;

                case (int)DBInUse.DARAJA_DB:
                  fromDarajaDB = results.Read<MobileWithdrawals_Daraja>();
                  darajaDBRecordCount = results.Read<int>().First();
                  darajaDBLastPage = (int)Math.Ceiling(darajaDBRecordCount / (decimal)pagingParams.PageSize);
                  break;
              }
            }
          }

        }

        if (saccoDBLastPage > darajaDBLastPage)
        {
          lastPage = saccoDBLastPage;
        }
        else if (darajaDBLastPage > saccoDBLastPage)
        {
          lastPage = darajaDBLastPage;
        }
        else
        {
          // both recordsets are equal
          lastPage = saccoDBLastPage;
        }
      }
      else
      {
        for (int i = 1; i <= 2; i++)
        {
          switch (i)
          {
            case (int)DBInUse.SACCO_DB:
              _tblMobileWithdrawals = MobileWithdrawals.DBTableName;
              _connString = _saccoDBConn;

              query = $@"SELECT * FROM {_tblMobileWithdrawals} 
                          WHERE [Corporate No]=@CorporateNo AND datediff(dd, [Transaction Date], getdate()) = 0
                          ORDER BY [Entry No] DESC";
              fromSACCODB = new DapperORM(_connString).QueryGetList<MobileWithdrawals>(query, qryParams);
              break;

            case (int)DBInUse.DARAJA_DB:
              _tblMobileWithdrawals = DarajaDB_MobileWithdrawals.TableName;
              _connString = _darajaDBConn;

              query = $@"SELECT * FROM {_tblMobileWithdrawals} 
                          WHERE [Corporate No]=@CorporateNo 
                          AND (
                            select 
                              case 
                                when [Transaction Date] is not null
                                then datediff(dd, [Transaction Date], getdate())
                                else datediff(dd, [Datetime], getdate())
                              end) = 0
                          ORDER BY [Entry No] DESC";
              fromDarajaDB = new DapperORM(_connString).QueryGetList<MobileWithdrawals_Daraja>(query, qryParams);
              break;
          }
        }
      }
      // return recordset in format of old db because it is the current stable and trusted platform 10Jul2020_1711
      IEnumerable<IMobileWithdrawals_SACCODB> records = (
        fromSACCODB ?? Enumerable.Empty<IMobileWithdrawals_SACCODB>()).Concat(
        Mapper.Map<IEnumerable<IMobileWithdrawals_DarajaDB>, IEnumerable<IMobileWithdrawals_SACCODB>>(fromDarajaDB) ?? Enumerable.Empty<IMobileWithdrawals_SACCODB>());

      return records;
    }

    public IMobileWithdrawals_SACCODB GetLatestWithdrawalForClient(string corporateNo)
    {
      IMobileWithdrawals_SACCODB fromSACCODB = null;
      IMobileWithdrawals_DarajaDB fromDarajaDB = null;

      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      for (int i = 1; i <= 2; i++)
      {
        switch (i)
        {
          case (int)DBInUse.SACCO_DB:
            _tblMobileWithdrawals = MobileWithdrawals.DBTableName;
            _connString = _saccoDBConn;
            break;

          case (int)DBInUse.DARAJA_DB:
            _tblMobileWithdrawals = DarajaDB_MobileWithdrawals.TableName;
            _connString = _darajaDBConn;
            break;
        }

        query = $@"SELECT TOP 1 *
                  FROM {_tblMobileWithdrawals}
                  WHERE [Corporate No] = @CorporateNo
                  AND Status='Completed' 
                  AND [MPESA Result Code]='0' 
                  AND [MPESA Result Type]='Completed'
                  ORDER BY [Transaction Date] DESC";


        switch (i)
        {
          case (int)DBInUse.SACCO_DB:
            fromSACCODB = new DapperORM(_connString).QueryGetSingle<MobileWithdrawals>(query, qryParams);
            break;
          case (int)DBInUse.DARAJA_DB:
            fromDarajaDB = new DapperORM(_connString).QueryGetSingle<MobileWithdrawals_Daraja>(query, qryParams);
            break;
        }
      }

      IMobileWithdrawals_SACCODB returnRecord = null;
      if (fromSACCODB != null && fromDarajaDB != null)
      {
        if (fromSACCODB.Transaction_Date > fromDarajaDB.Transaction_Date)
        {
          returnRecord = fromSACCODB;
        }
        else if (fromDarajaDB.Transaction_Date > fromSACCODB.Transaction_Date)
        {
          returnRecord = Mapper.Map<IMobileWithdrawals_DarajaDB, IMobileWithdrawals_SACCODB>(fromDarajaDB);
        }
        else
        {
          // Both transaction dates are equal preference is to return from old db
          returnRecord = fromSACCODB;
        }
      }
      else if (fromSACCODB != null)
      {
        returnRecord = fromSACCODB;
      }
      else if (fromDarajaDB != null)
      {
        returnRecord = Mapper.Map<IMobileWithdrawals_DarajaDB, IMobileWithdrawals_SACCODB>(fromDarajaDB);
      }
      else
      {
        returnRecord = null;
      }

      return returnRecord;
    }

  }
}
