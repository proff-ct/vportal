using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.FluentMap;
using VisibilityPortal_BLL.Models.Mappings;

namespace VisibilityPortal_BLL
{

  public class DapperORM
  {
    //private static string _connectionString = @"Data Source=MATT-HP-PAV-450;Initial Catalog=DCS;Integrated Security=True";
#if DEBUG
    private static string _connectionString = Connection.TestingConnectionString;
#else
    private static string _connectionString = Connection.ProductionConnectionString;
#endif

    public DapperORM(string conString=null)
    {
      _connectionString = conString?? _connectionString;
      InitDapper();
    }
    private static void InitDapper()
    {
      FluentMapper.Initialize(config =>
      {
        config.AddMap(new SaccoMap());
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
        return sqlCon.Query<T>(query, commandType: CommandType.Text);
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
