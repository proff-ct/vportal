using System.Collections.Generic;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ViewModels;
using VisibilityPortal_BLL.Utilities.MSSQLOperators;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL
{
  public interface IBL_CoretecClient
  {
    bool Delete(string moduleId);
    IEnumerable<PortalModuleForClient> GetAllPortalModulesForClient(string clientCorporateNo);
    IEnumerable<CoretecClientModuleViewModel> GetListOfClientModules(out int lastPage, bool paginate = false, PaginationParameters pagingParams = null);
    IEnumerable<CoretecClientWithModule> GetListOfClientsWithModules(out int lastPage, bool paginate = false, PaginationParameters pagingParams = null);
    PortalModuleForClient GetPortalModuleForClient(string clientModuleId);
    IEnumerable<CoreTecClient> GetRegisteredClients();
    IEnumerable<CoreTecClient> GetUnregisteredClients();
    bool Save(PortalModuleForClient clientModule, ModelOperation modelOp);
  }
}