using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.FluentMap;
using MSacco_BLL.Models.Mappings;

namespace MSacco_BLL
{

  public class DapperORM
  {
    //private static string _connectionString = @"Data Source=MATT-HP-PAV-450;Initial Catalog=DCS;Integrated Security=True";
#if DEBUG
    private static string _connectionString = Connection.TestingConnectionString;
#else
    private static string _connectionString = Connection.ProductionConnectionString;
#endif

    public DapperORM(string conString = null)
    {
      _connectionString = conString ?? _connectionString;
      //InitDapper();
    }
    static DapperORM()
    {
      InitDapper();
    }
    
    private static void InitDapper()
    {
      FluentMapper.Initialize(config =>
      {
        config.AddMap(new SaccoMap());
        config.AddMap(new MSaccoSalaryAdvanceMap());
        //config.AddMap(new GFLLoanMap());
        config.AddMap(new GuarantorsMap());
        config.AddMap(new MSaccoUtilityPaymentMap());
        config.AddMap(new MSaccoAirtimeTopupMap());
        config.AddMap(new MobileWithdrawalsMap());
        config.AddMap(new LinkMonitoringMap());
        config.AddMap(new LinkDowntimeMap());
        config.AddMap(new ArchivedBulkSMSDebitMap());
        config.AddMap(new UnarchivedBulkSMSDebitMap());
        config.AddMap(new BulkSMSBalanceMap());
        config.AddMap(new WauminiVirtualRegistrationIPRSMap());
        config.AddMap(new VirtualRegistrationNewIPRSMap());
      });
    }
    public string ConnectionString { get => _connectionString; }
    public T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null)
    {
      using (SqlConnection sqlCon = new SqlConnection(_connectionString))
      {
        sqlCon.Open();
        return (T)Convert.ChangeType(
          sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure),
          typeof(T)
        );
      }
    }
    public void ExecuteWithoutReturn(string procedureName, DynamicParameters param)
    {
      using (SqlConnection sqlCon = new SqlConnection(_connectionString))
      {
        sqlCon.Open();
        sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
      }
    }

    public IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
    {
      using (SqlConnection sqlCon = new SqlConnection(_connectionString))
      {
        sqlCon.Open();
        return sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
      }
    }

    public IEnumerable<T> QueryGetList<T>(string query)
    {
      using (SqlConnection sqlCon = new SqlConnection(_connectionString))
      {
        sqlCon.Open();
        return sqlCon.Query<T>(
          query, commandType: CommandType.Text, commandTimeout: sqlCon.ConnectionTimeout);
      }
    }
    public T QueryGetSingle<T>(string query)
    {
      using (SqlConnection sqlCon = new SqlConnection(_connectionString))
      {
        sqlCon.Open();
        return sqlCon.QuerySingleOrDefault<T>(query, commandType: CommandType.Text);
      }
    }
    public void ExecuteQuery(string query)
    {
      using (SqlConnection sqlCon = new SqlConnection(_connectionString))
      {
        sqlCon.Open();
        sqlCon.Execute(query, commandType: CommandType.Text);
      }
    }
  }
}
