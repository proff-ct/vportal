using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CallCenter_DAL;
using CallCenter_Dataspecs.Functions;
using CallCenter_Dataspecs.Models;
using CallCenter_Dataspecs.MSACCOCustomer.Functions;
using CallCenter_Dataspecs.MSACCOCustomer.Models;
using CallCenter_Dataspecs.MSSQLOperators;
using CallCenter_Dataspecs.USSDRequests.Functions;
using CallCenter_Dataspecs.USSDRequests.Models;
using Dapper;
using Newtonsoft.Json;
using VisibilityPortal_Dataspecs.Models;

namespace CallCenter_BLL
{
  public class USSDRequestLogBLL : IBL_USSDRequestLog
  {
    private string _query;
    private readonly string _tblUSSDRequestsLog = USSDRequest.DBTableName;

    public IEnumerable<IUSSD_Request_ViewModel> GetUSSDRequestLogsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblUSSDRequestsLog} 
          WHERE [Corporate No] = '{corporateNo}'
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblUSSDRequestsLog}
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
        _query = $@"SELECT TOP 1000 * FROM {_tblUSSDRequestsLog}
                  WHERE [Corporate No] = '{corporateNo}' 
                  ORDER BY [Entry No] DESC";
        return USSDRequests_BL.ParseUSSDRequests(new DapperORM().QueryGetList<USSDRequest>(_query));
      }

    }

    public IEnumerable<IUSSD_Request_ViewModel> GetMemberUSSDRequestLogForClient(string corporateNo, string memberPhoneNo)
    {

      _query = $@"SELECT TOP 1000 * FROM {_tblUSSDRequestsLog} 
                WHERE [Corporate No] = '{corporateNo}' AND [Telephone No] = '{memberPhoneNo}'
                ORDER BY [Entry No] DESC";

      return USSDRequests_BL.ParseUSSDRequests(new DapperORM().QueryGetList<USSDRequest>(_query));
    }

  }
}
