using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_BLL.Utilities;
using CallCenter_DAL;
using Dapper;

namespace CallCenter_BLL
{
  public class FloatResourceAlertForClientBLL
  {
#if DEBUG
    private readonly string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
#else
    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
#endif

    private string _query;
    private readonly string _tblFloatResourceAlertForClient = FloatResourceAlertForClient.DBTableName;

    public IEnumerable<FloatResourceAlertForClient> GetListOfFloatResourceAlertsForAllClients(
      out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblFloatResourceAlertForClient} 
          ORDER BY CreatedOn DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count(Id) as TotalRecords  
          FROM {_tblFloatResourceAlertForClient}
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(
          new DapperORM(_connString).ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
          {
            IEnumerable<FloatResourceAlertForClient> loans = results.Read<FloatResourceAlertForClient>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return loans;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblFloatResourceAlertForClient}";
        return new DapperORM(_connString).QueryGetList<FloatResourceAlertForClient>(_query);
      }

    }

    public IEnumerable<FloatResourceAlertForClient> GetListOfFloatResourceAlertsForClient(
      string clientCorporateNo)
    {
      _query = $@"SELECT * FROM {_tblFloatResourceAlertForClient} WHERE ClientCorporateNo='{clientCorporateNo}'";
      return new DapperORM(_connString).QueryGetList<FloatResourceAlertForClient>(_query);
    }

    public FloatResourceAlertForClient GetFloatResourceAlertForClientById(int Id)
    {
      _query = $@"SELECT * FROM {_tblFloatResourceAlertForClient} WHERE Id='{Id}'";
      return new DapperORM(_connString).QueryGetSingle<FloatResourceAlertForClient>(_query);
    }


    public bool Save(FloatResourceAlertForClient clientFRA, ModelOperation modelOp)
    {
      if (!ValidateFloatResourceAlertForClient(clientFRA))
      {
        return false;
      }

      switch (modelOp)
      {
        case ModelOperation.AddNew:
          _query = $@"INSERT INTO {_tblFloatResourceAlertForClient}
                  (ClientCorporateNo,
                  FloatResourceId,
                  AlertTypeId,
                  Threshold,
                  TriggerCondition,
                  CreatedBy)
                  VALUES(
                  '{clientFRA.ClientCorporateNo}',
                  '{clientFRA.FloatResourceId}',
                  '{clientFRA.AlertTypeId}',
                  '{clientFRA.Threshold}',
                  '{clientFRA.TriggerCondition}',
                  '{clientFRA.CreatedBy}')
                ";
          break;

        case ModelOperation.Update:
          _query = $@"UPDATE {_tblFloatResourceAlertForClient} 
          SET
            Threshold='{clientFRA.Threshold}',
            TriggerCondition='{clientFRA.TriggerCondition}',
            ModifiedBy='{clientFRA.ModifiedBy}',
            ModifiedOn='{DateTime.UtcNow}'
          WHERE Id='{clientFRA.Id}'";
          break;
      };
      try
      {
        new DapperORM(_connString).ExecuteQuery(_query);
        return true;
      }
      catch (Exception)
      {
        // should log the exception
        return false;
      }
    }
    public bool Delete(string clientFRAId)
    {
      _query = $@"DELETE FROM {_tblFloatResourceAlertForClient} WHERE Id='{clientFRAId}'";
      try
      {
        new DapperORM(_connString).ExecuteQuery(_query);
        return true;
      }
      catch (Exception)
      {
        // log the exception
        return false;
      }
    }
    private bool ValidateFloatResourceAlertForClient(FloatResourceAlertForClient clientFRA)
    {
      return Validators.ValidateObject(clientFRA);
    }

  }
}
