using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using CallCenter_BLL.Models;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_DAL;
using Dapper;

namespace CallCenter_BLL
{
  public class LinkMonitoringBLL
  {
    private string _query;
    private readonly string _tblLinkMonitoring = LinkMonitoring.DBTableName;
    public LinkMonitoring GetLinkInfoForClient(string corporateNo)
    {
      _query = $@"SELECT * FROM {_tblLinkMonitoring} WHERE [Corporate No]='{corporateNo}'";
      return new DapperORM().QueryGetSingle<LinkMonitoring>(_query);
    }

    public IEnumerable<LinkMonitoring> GetLinkInfoForAllClients(
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblLinkMonitoring} 
          ORDER BY [Corporate Name] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Corporate Name]) as TotalRecords  
          FROM {_tblLinkMonitoring}
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<LinkMonitoring> records = results.Read<LinkMonitoring>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblLinkMonitoring}";
        return new DapperORM().QueryGetList<LinkMonitoring>(_query);
      }

    }
    public IEnumerable<LinkStatusPlusDowntime> GetLinkInfoWithLinkDowntimeForAllClients(
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      List<LinkStatusPlusDowntime> linkStatusRecords = new List<LinkStatusPlusDowntime>();

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblLinkMonitoring} 
          ORDER BY [Corporate Name] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Corporate Name]) as TotalRecords  
          FROM {_tblLinkMonitoring}
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<LinkMonitoring> records = results.Read<LinkMonitoring>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);

            // r
            records.ToList().ForEach(r =>
            {
              linkStatusRecords.Add(GetLinkInfoWithLinkDowntimeForClient(r.Corporate_No, r));
            });
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblLinkMonitoring}";
        IEnumerable<LinkMonitoring> linkInfoRecords = new DapperORM().QueryGetList<LinkMonitoring>(_query);

        linkInfoRecords.ToList().ForEach(r =>
        {
          linkStatusRecords.Add(GetLinkInfoWithLinkDowntimeForClient(r.Corporate_No, r));
        });
      }

      return linkStatusRecords.AsEnumerable();
    }
    public LinkStatusPlusDowntime GetLinkInfoWithLinkDowntimeForClient(
      string corporateNo, LinkMonitoring linkInfo = null)
    {

      string tblLinkDowntime = LinkDowntime.DBTableName;
      LinkDowntimeBLL linkDowntimeBLL = new LinkDowntimeBLL();

      linkInfo = linkInfo ?? GetLinkInfoForClient(corporateNo);
      IEnumerable<LinkDowntime> linkDowntimes = linkDowntimeBLL.GetDowntimeRecordsForClient(
        corporateNo);

      LinkStatusPlusDowntime linkStatusPlusDowntime = Mapper.Map<LinkStatusPlusDowntime>(linkInfo);
      linkStatusPlusDowntime.Downtimes = linkDowntimes;

      return linkStatusPlusDowntime;
    }
  }
}
