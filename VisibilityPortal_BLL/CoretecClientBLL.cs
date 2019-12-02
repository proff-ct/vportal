using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using Dapper;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ViewModels;
using VisibilityPortal_BLL.Utilities;
using VisibilityPortal_BLL.Utilities.MSSQLOperators;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL
{
  public class CoretecClientBLL
  {
    private string _query;
    private readonly string _tblClientModule = PortalModuleForClient.DBTableName;
    private readonly SaccoBLL _saccoBLL = new SaccoBLL();

    public IEnumerable<CoretecClientWithModule> GetListOfClientsWithModules(out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      List<PortalModuleForClient> listPortalModuleForClient = new List<PortalModuleForClient>();
      List<Sacco> listSacco = new List<Sacco>();
      List<CoreTecClient> listCoretecClient = new List<CoreTecClient>();
      List<CoretecClientWithModule> listCoretecClientWithModule = new List<CoretecClientWithModule>();

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblClientModule}
          ORDER BY ClientCorporateNo ASC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count(ClientCorporateNo) as TotalRecords  
          FROM {_tblClientModule}
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (
            SqlMapper.GridReader results = sqlCon.QueryMultiple(
              _query, dp, commandType: CommandType.Text))
          {
            IEnumerable<PortalModuleForClient> records = results.Read<PortalModuleForClient>();
            int totalRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalRecords / (decimal)pagingParams.PageSize);
            listPortalModuleForClient = records.ToList();
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblClientModule}";
        listPortalModuleForClient = new DapperORM()
          .QueryGetList<PortalModuleForClient>(_query).ToList();
      }
                           
      listPortalModuleForClient.Select(r=>r.ClientCorporateNo).Distinct().ToList().ForEach(cNo => {
        listSacco.Add(_saccoBLL.GetSaccoByUniqueParam(cNo));
      });

      listCoretecClient = Mapper.Map<IEnumerable<CoreTecClient>>(listSacco).ToList();
      listCoretecClientWithModule = Mapper.Map<List<CoretecClientWithModule>>(listCoretecClient);

      listCoretecClientWithModule.ForEach(c => {
        c.Modules = listPortalModuleForClient.Where(m => m.ClientCorporateNo == c.corporateNo);
      });

      return listCoretecClientWithModule.AsEnumerable();
    }

    public IEnumerable<CoretecClientModuleViewModel> GetListOfClientModules(out int lastPage,
      bool paginate = false,
      PaginationParameters pagingParams = null)
    {
      lastPage = 0;
      List<PortalModuleForClient> listPortalModuleForClient = new List<PortalModuleForClient>();
      List<CoretecClientModuleViewModel> listCoretecClientModule = new List<CoretecClientModuleViewModel>();
      List<Sacco> listSacco = new List<Sacco>();

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblClientModule}
          ORDER BY ClientCorporateNo ASC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count(ClientCorporateNo) as TotalRecords  
          FROM {_tblClientModule}
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (
            SqlMapper.GridReader results = sqlCon.QueryMultiple(
              _query, dp, commandType: CommandType.Text))
          {
            IEnumerable<PortalModuleForClient> records = results.Read<PortalModuleForClient>();
            int totalRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalRecords / (decimal)pagingParams.PageSize);
            listPortalModuleForClient = records.ToList();
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblClientModule}";
        listPortalModuleForClient = new DapperORM()
          .QueryGetList<PortalModuleForClient>(_query).ToList();
      }
      listCoretecClientModule = Mapper.Map<List<CoretecClientModuleViewModel>>(listPortalModuleForClient);
      if (listCoretecClientModule.Count == 0) return listCoretecClientModule.AsEnumerable();
      
      // get names of saccos for each distinct CorporateNo from the returned data
      listPortalModuleForClient.Select(r => r.ClientCorporateNo).Distinct().ToList().ForEach(cNo => {
        listSacco.Add(_saccoBLL.GetSaccoByUniqueParam(cNo));
      });

      listCoretecClientModule.ForEach(m => {
        m.SaccoName = listSacco.First(s => s.corporateNo == m.ClientCorporateNo).saccoName_1;
      });

      return listCoretecClientModule.AsEnumerable();
    }

    public IEnumerable<CoreTecClient> GetUnregisteredClients()
    {
      List<CoreTecClient> listCurrentClients = new List<CoreTecClient>();
      List<CoreTecClient> listRegisteredClients = new List<CoreTecClient>();
      List<string> listRegisteredClientsCorporateNos = new List<string>();

      _query = $@"SELECT * FROM {_tblClientModule}";
      listRegisteredClientsCorporateNos = new DapperORM()
        .QueryGetList<PortalModuleForClient>(_query)
        .ToList()
        .Select(r => r.ClientCorporateNo).Distinct().ToList();

      listCurrentClients = Mapper.Map<List<CoreTecClient>>(_saccoBLL.GetSaccoList().ToList());
      return listCurrentClients
        .Where(c => listRegisteredClientsCorporateNos.All(cNo => cNo != c.corporateNo));
    }
    public IEnumerable<CoreTecClient> GetRegisteredClients()
    {
      List<CoreTecClient> listCurrentClients = new List<CoreTecClient>();
      List<CoreTecClient> listRegisteredClients = new List<CoreTecClient>();
      List<string> listRegisteredClientsCorporateNos = new List<string>();

      _query = $@"SELECT * FROM {_tblClientModule}";
      listRegisteredClientsCorporateNos = new DapperORM()
        .QueryGetList<PortalModuleForClient>(_query)
        .ToList()
        .Select(r => r.ClientCorporateNo).Distinct().ToList();

      listCurrentClients = Mapper.Map<List<CoreTecClient>>(_saccoBLL.GetSaccoList().ToList());
      return listCurrentClients
        .Where(c => listRegisteredClientsCorporateNos.Contains(c.corporateNo));
    }

    public PortalModuleForClient GetPortalModuleForClient(string clientModuleId)
    {
      _query= $@"SELECT * FROM {_tblClientModule} WHERE ClientModuleId='{clientModuleId}'";

      return new DapperORM().QueryGetSingle<PortalModuleForClient>(_query);
    }
    public bool Save(PortalModuleForClient clientModule, ModelOperation modelOp)
    {
      if (!ValidatePortalModuleForClient(clientModule)) return false;
      switch (modelOp)
      {
        case ModelOperation.AddNew:
          _query = $@"INSERT INTO {_tblClientModule}
                  (ClientModuleId,
                  ClientCorporateNo,
                  PortalModuleName,
                  CreatedBy,
                  IsEnabled)
                  VALUES
                  ('{Guid.NewGuid()}',
                  '{clientModule.ClientCorporateNo}',
                  '{clientModule.PortalModuleName}',
                  '{clientModule.CreatedBy}',
                  '{1}')
                ";
          break;

        case ModelOperation.Update:
          _query = $@"UPDATE {_tblClientModule} 
          SET
            IsEnabled='{clientModule.IsEnabled}',
            ModifiedBy='{clientModule.ModifiedBy}'
          WHERE ClientModuleId='{clientModule.ClientModuleId}'";
          break;
      };
      try
      {
        new DapperORM().ExecuteQuery(_query);
        return true;
      }
      catch (Exception ex)
      {
        // should log the exception
        return false;
      }
    }
    public bool Delete(string moduleId)
    {
      _query = $@"DELETE FROM {_tblClientModule} WHERE ClientModuleId='{moduleId}'";
      try
      {
        new DapperORM().ExecuteQuery(_query);
        return true;
      }
      catch (Exception ex)
      {
        // log the exception
        return false;
      }
    }

    private bool ValidatePortalModuleForClient(PortalModuleForClient clientModule)
    {
      return Validators.ValidateObject(clientModule);
    }
  }
}
