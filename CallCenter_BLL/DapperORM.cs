using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.FluentMap;
using CallCenter_BLL.Models.Mappings;

namespace CallCenter_BLL
{

  public static class DapperORM
  {
    //private static string connectionString = @"Data Source=MATT-HP-PAV-450;Initial Catalog=DCS;Integrated Security=True";
#if DEBUG
    private static string connectionString = Connection.TestingConnectionString;
#else
    private static string connectionString = Connection.ProductionConnectionString;
#endif

    //private static string connectionString = Connection.ProductionConnectionString;
    static DapperORM()
    {
      ConnectionString = connectionString;
      FluentMapper.Initialize(config =>
      {
        config.AddMap(new SaccoMap());
        config.AddMap(new MSaccoSalaryAdvanceMap());
        //config.AddMap(new GFLLoanMap());
        config.AddMap(new GuarantorsMap());
        config.AddMap(new MSaccoUtilityPaymentMap());
      });
    }
    public static string ConnectionString { get; }
    public static T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null)
    {
      using (SqlConnection sqlCon = new SqlConnection(connectionString))
      {
        sqlCon.Open();
        return (T)Convert.ChangeType(
          sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure),
          typeof(T)
        );
      }
    }
    public static void ExecuteWithoutReturn(string procedureName, DynamicParameters param)
    {
      using (SqlConnection sqlCon = new SqlConnection(connectionString))
      {
        sqlCon.Open();
        sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
      }
    }

    public static IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
    {
      using (SqlConnection sqlCon = new SqlConnection(connectionString))
      {
        sqlCon.Open();
        return sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
      }
    }

    public static IEnumerable<T> QueryGetList<T>(string query)
    {
      using (SqlConnection sqlCon = new SqlConnection(connectionString))
      {
        sqlCon.Open();
        return sqlCon.Query<T>(query, commandType: CommandType.Text);
      }
    }
    public static T QueryGetSingle<T>(string query)
    {
      using (SqlConnection sqlCon = new SqlConnection(connectionString))
      {
        sqlCon.Open();
        return sqlCon.QuerySingleOrDefault<T>(query, commandType: CommandType.Text);
      }
    }
    public static void ExecuteQuery(string query)
    {
      using (SqlConnection sqlCon = new SqlConnection(connectionString))
      {
        sqlCon.Open();
        sqlCon.Execute(query, commandType: CommandType.Text);
      }
    }
  }
}
