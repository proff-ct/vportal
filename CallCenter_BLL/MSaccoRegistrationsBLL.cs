using CallCenter_DAL;
using CallCenter_Dataspecs.MSACCOCustomer.Functions;
using CallCenter_Dataspecs.MSACCOCustomer.Models;
using CallCenter_Dataspecs.MSSQLOperators;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_BLL
{
  public class MSaccoRegistrationsBLL : IBL_MsaccoRegistration
  {
    private string _query;
    private readonly string _tblMsaccoRegsitrationRecords = RoutingTable.DBTableName;
    public IEnumerable<IRouting_Table> GetMsaccoRegistrationListForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}] 
          WHERE [Corporate No]='{corporateNo}'
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoRegsitrationRecords}
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
            IEnumerable<IRouting_Table> records = results.Read<RoutingTable>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}] WHERE [Corporate No]='{corporateNo}' ORDER BY [Entry No] DESC";
        return new DapperORM().QueryGetList<RoutingTable>(_query);
      }
    }

    public IRouting_Table GetMsaccoRegistrationRecordForClient(string corporateNo, string MemberPhoneNo)
    {
      _query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}]
                WHERE [Corporate No]='{corporateNo}' AND [Telephone No]='{MemberPhoneNo}'
                ORDER BY [Entry No] DESC";
      return new DapperORM().QueryGetSingle<RoutingTable>(_query);
    }
  }
}
