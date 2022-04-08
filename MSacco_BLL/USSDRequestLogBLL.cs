using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.USSDRequestLogging.Functions;
using MSacco_Dataspecs.Feature.USSDRequestLogging.Models;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL
{
    public class USSDRequestLogBLL : IBL_USSDRequestLog
    {
        private readonly string _tblUSSDRequestsLog = USSDRequest.DBTableName;

        public IEnumerable<IUSSD_Request_ViewModel> GetUSSDRequestLogsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
        {
            lastPage = 0;
            DynamicParameters qryParams = new DynamicParameters();
            qryParams.Add("CorporateNo", corporateNo);
            string query;

            if (paginate)
            {
                query = $@"SELECT * FROM {_tblUSSDRequestsLog} 
          WHERE [Corporate No] = @CorporateNo
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblUSSDRequestsLog}
          WHERE [Corporate No]=@CorporateNo
          ";

                qryParams.Add("PageSize", pagingParams.PageSize);
                qryParams.Add("PageNumber", pagingParams.PageToLoad);

                using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
                {
                    sqlCon.Open();
                    using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
                    {
                        IEnumerable<IUSSDRequest> records = results.Read<USSDRequest>();
                        int totalRecords = results.Read<int>().First();

                        lastPage = (int)Math.Ceiling(
                          totalRecords / (decimal)pagingParams.PageSize);
                        return USSDRequests_BL.ParseUSSDRequests(records);
                    }
                }
            }
            else
            {
                query = $@"SELECT TOP 1000 * FROM {_tblUSSDRequestsLog}
                  WHERE [Corporate No] = @CorporateNo 
                  ORDER BY [Entry No] DESC";
                return USSDRequests_BL.ParseUSSDRequests(new DapperORM().QueryGetList<USSDRequest>(query, qryParams));
            }

        }

        public IEnumerable<IUSSD_Request_ViewModel> GetMemberUSSDRequestLogForClient(string corporateNo, string memberPhoneNo)
        {
            DynamicParameters qryParams = new DynamicParameters();
            qryParams.Add("CorporateNo", corporateNo);
            qryParams.Add("PhoneNo", memberPhoneNo);

            string query = $@"SELECT TOP 1000 * FROM {_tblUSSDRequestsLog} 
                WHERE [Corporate No] = @CorporateNo AND [Telephone No] = @PhoneNo
                ORDER BY [Entry No] DESC";

            return USSDRequests_BL.ParseUSSDRequests(new DapperORM().QueryGetList<USSDRequest>(query, qryParams));
        }

    }
}
