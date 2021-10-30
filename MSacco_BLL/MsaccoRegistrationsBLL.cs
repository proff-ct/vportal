using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Functions;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Models;
using MSacco_Dataspecs.MSSQLOperators;

namespace MSacco_BLL
{
  public class MsaccoRegistrationsBLL : IBL_MsaccoRegistration
  {
    private readonly string _newEnvironmentDBConn;
    private readonly string _saccoDBConn;
    private string _connString;
    private string _tblMsaccoRegsitrationRecords = Routing_Table.DBTableName;
    private enum DBInUse
    {
      SACCO_DB = 1,
      NEW_ENVIRONMENT_DB = 2
    }
    public MsaccoRegistrationsBLL()
    {
      _newEnvironmentDBConn = @ConfigurationManager.ConnectionStrings[MS_DBConnectionStrings.NewEnvironmentDBConnectionStringName].ConnectionString;
      _saccoDBConn = new DapperORM().ConnectionString;
    }

    public IEnumerable<IRouting_Table> GetMsaccoRegistrationListForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
    {
      lastPage = 0;
      int saccoDBLastPage = 0;
      int newEnvironmentDBLastPage = 0;

      IEnumerable<IRouting_Table> fromSACCODB = null;
      IEnumerable<ICustomer> fromNewEnvironmentDB = null;

      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      if (paginate)
      {
        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        int newEnvironmentRecordCount = 0;
        int saccoDBRecordCount = 0;

        for (int i = 1; i <= 2; i++)
        {
          switch (i)
          {
            case (int)DBInUse.SACCO_DB:
              _tblMsaccoRegsitrationRecords = Routing_Table.DBTableName;
              _connString = _saccoDBConn;
              break;

            case (int)DBInUse.NEW_ENVIRONMENT_DB:
              _tblMsaccoRegsitrationRecords = Customers.DBTableName;
              _connString = _newEnvironmentDBConn;
              break;
          }

          query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}] 
          WHERE [Corporate No]= @CorporateNo
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoRegsitrationRecords}
          WHERE [Corporate No]=@CorporateNo
          ";

          using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
          {
            sqlCon.Open();
            using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
            {
              //IEnumerable<IRouting_Table> records = results.Read<Routing_Table>();
              //  int totalLoanRecords = results.Read<int>().First();

              //  lastPage = (int)Math.Ceiling(
              //    totalLoanRecords / (decimal)pagingParams.PageSize);
              //  return records;
              switch (i)
              {
                case (int)DBInUse.SACCO_DB:
                  fromSACCODB = results.Read<Routing_Table>();
                  saccoDBRecordCount = results.Read<int>().First();
                  saccoDBLastPage = (int)Math.Ceiling(saccoDBRecordCount / (decimal)pagingParams.PageSize);
                  break;

                case (int)DBInUse.NEW_ENVIRONMENT_DB:
                  fromNewEnvironmentDB = results.Read<Customers>();
                  newEnvironmentRecordCount = results.Read<int>().First();
                  newEnvironmentDBLastPage = (int)Math.Ceiling(newEnvironmentRecordCount / (decimal)pagingParams.PageSize);
                  break;
              }

            }
          }
        }

        if (saccoDBLastPage > newEnvironmentDBLastPage)
        {
          lastPage = saccoDBLastPage;
        }
        else if (newEnvironmentDBLastPage > saccoDBLastPage)
        {
          lastPage = newEnvironmentDBLastPage;
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
              _tblMsaccoRegsitrationRecords = Routing_Table.DBTableName;
              _connString = _saccoDBConn;
              query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}] WHERE [Corporate No]=@CorporateNo ORDER BY [Entry No] DESC";

              fromSACCODB = new DapperORM(_connString).QueryGetList<Routing_Table>(query, qryParams);
              break;

            case (int)DBInUse.NEW_ENVIRONMENT_DB:
              _tblMsaccoRegsitrationRecords = Customers.DBTableName;
              _connString = _newEnvironmentDBConn;
              query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}] WHERE [Corporate No]=@CorporateNo ORDER BY [Entry No] DESC";

              fromNewEnvironmentDB = new DapperORM(_connString).QueryGetList<Customers>(query, qryParams);
              break;
          }
        }
      }
      IEnumerable<IRouting_Table> records = null;
      records = fromSACCODB ?? Mapper.Map<IEnumerable<IRouting_Table>>(fromNewEnvironmentDB);

      return records;
    }

    public IRouting_Table GetMsaccoRegistrationRecordForClient(string corporateNo, string MemberPhoneNo)
    {
      IRouting_Table fromSACCODB = null;
      ICustomer fromNewEnvironmentDB = null;

      IRouting_Table returnRecord = null;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      for (int i = 1; i <= 2; i++)
      {
        switch (i)
        {
          case (int)DBInUse.SACCO_DB:
            _tblMsaccoRegsitrationRecords = Routing_Table.DBTableName;
            _connString = _saccoDBConn;
            break;

          case (int)DBInUse.NEW_ENVIRONMENT_DB:
            _tblMsaccoRegsitrationRecords = Customers.DBTableName;
            _connString = _newEnvironmentDBConn;
            break;
        }
        qryParams.Add("CorporateNo", corporateNo);
        qryParams.Add("PhoneNo", MemberPhoneNo);

        query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}]
                WHERE [Corporate No]=@CorporateNo AND [Telephone No]=@PhoneNo
                ORDER BY [Entry No] DESC";

        switch (i)
        {
          case (int)DBInUse.SACCO_DB:
            fromSACCODB = new DapperORM(_connString).QueryGetSingle<Routing_Table>(query, qryParams);
            break;
          case (int)DBInUse.NEW_ENVIRONMENT_DB:
            fromNewEnvironmentDB = new DapperORM(_connString).QueryGetSingle<Customers>(query, qryParams);
            break;
        }
      }
      if (fromSACCODB != null)
      {
        returnRecord = fromSACCODB;
      }
      else if (fromNewEnvironmentDB != null)
      {
        returnRecord = Mapper.Map<ICustomer, IRouting_Table>(fromNewEnvironmentDB);
      }
      else
      {
        returnRecord = null;
      }

      return returnRecord;
    }
  }
}
