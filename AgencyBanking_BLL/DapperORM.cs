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

        public static IEnumerable<T> QueryGetList<T>(string query, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(query,param, commandType: CommandType.Text);
            }
        }
        public static T QueryGetSingle<T>(string query,DynamicParameters parameters =null)
        {

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.QuerySingleOrDefault<T>(query, parameters,commandType: CommandType.Text);
            }


        } 
        public static T QueryGetSingle<T>(string conn,string query,DynamicParameters parameters =null)
        {

            using (SqlConnection sqlCon = new SqlConnection(conn))
            {
                sqlCon.Open();
                return sqlCon.QuerySingleOrDefault<T>(query, parameters,commandType: CommandType.Text);
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

        public static int ExecuteInsert(string query, DynamicParameters parameters = null)
        {
            int affectedrows = -1;
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
               affectedrows =  sqlCon.Execute(query,parameters);
            }

            return affectedrows;

        }
    }
}
