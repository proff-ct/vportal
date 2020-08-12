using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Functions;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Models;
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
  public class MsaccoRegistrationsBLL : IBL_MsaccoRegistration
  {
    private string _query;
    private readonly string _tblMsaccoRegsitrationRecords = Routing_Table.DBTableName;
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
            IEnumerable<IRouting_Table> records = results.Read<Routing_Table>();
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
        return new DapperORM().QueryGetList<Routing_Table>(_query);
      }
    }

    public IRouting_Table GetMsaccoRegistrationRecordForClient(string corporateNo, string MemberPhoneNo)
    {
      _query = $@"SELECT * FROM [{_tblMsaccoRegsitrationRecords}]
                WHERE [Corporate No]='{corporateNo}' AND [Telephone No]='{MemberPhoneNo}'
                ORDER BY [Entry No] DESC";
      return new DapperORM().QueryGetSingle<Routing_Table>(_query);
    }
  }
}
