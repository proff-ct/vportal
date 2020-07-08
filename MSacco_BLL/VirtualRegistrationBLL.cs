using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MSacco_DAL;
using Dapper;
using MSacco_Dataspecs.Feature.VirtualRegistration.Functions;
using MSacco_Dataspecs.Feature.VirtualRegistration.Models;
using MSacco_Dataspecs.MSSQLOperators;

namespace MSacco_BLL
{
  public class VirtualRegistrationBLL : IBL_VirtualRegistration
  {
    private string _query;
    private readonly string _tblVirtualRegistration = VirtualRegistrationNewIPRS.DBTableName;
    public IEnumerable<IVirtualRegistrationIPRS> GetIPRSVirtualRegistrationListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      IPaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblVirtualRegistration} 
          WHERE [Corporate No]='{corporateNo}'
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblVirtualRegistration}
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
            IEnumerable<IVirtualRegistrationIPRS> records = results.Read<VirtualRegistrationNewIPRS>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblVirtualRegistration} WHERE [Corporate No]='{corporateNo}' ORDER BY [Entry No] DESC";
        return new DapperORM().QueryGetList<VirtualRegistrationNewIPRS>(_query);
      }

    }
     
  }
}
