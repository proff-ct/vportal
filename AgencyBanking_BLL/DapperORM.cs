using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using AgencyBanking_DAL;

namespace AgencyBanking_BLL
{
    class DapperOrm
    {
        private static readonly string ConnectionString = new DBConnection().ConnectionString;
        public static T ExecuteReturnScalar<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
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
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public static IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public static IEnumerable<T> QueryGetList<T>(string query)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(query, commandType: CommandType.Text);
            }
        }
        public static T QueryGetSingle<T>(string query)
        {

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.QuerySingleOrDefault<T>(query, commandType: CommandType.Text);
            }


        }
        public static void ExecuteQuery(string query)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(query, commandType: CommandType.Text);
            }
        }
    }
}
