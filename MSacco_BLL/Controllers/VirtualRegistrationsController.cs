using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MSacco_BLL.CustomFilters;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.VirtualRegistration.Functions;
using MSacco_Dataspecs.MSSQLOperators;
using MSacco_Dataspecs.Security;
using Newtonsoft.Json;
using Utilities.PortalApplicationParams;

namespace MSacco_BLL.Controllers
{
  [RequireActiveUserSession]
  public class VirtualRegistrationsController : Controller
  {
    private IBL_VirtualRegistration _virtualRegistrationBLL = new VirtualRegistrationBLL();
    // GET: VirtualRegistrations
    public ActionResult Index()
    {
      ViewBag.ActiveUserParams = (ActiveUserParams)Session["ActiveUserParams"]; 
      return View();
    }

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetvirtualRegistrationRecords(string clientCorporateNo, int page, int size)
    {
      ActiveUserParams userParams = (ActiveUserParams)Session["ActiveUserParams"];
      if (userParams == null || string.IsNullOrEmpty(clientCorporateNo))
      {
        return null;
      }

      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function


      //PaginationParameters pagingParams = new PaginationParameters(
      //  int.Parse(page), int.Parse(size), null);
      IPaginationParameters pagingParams = new PaginationParameters(page, size, null);

      dynamic virtualRegistrationRecords = _virtualRegistrationBLL
        .GetIPRSVirtualRegistrationListForClient(clientCorporateNo, out int lastPage, true, pagingParams)
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = APICommunication.Encrypt(
          JsonConvert.SerializeObject(virtualRegistrationRecords),
          new MSACCO_AES(userParams.APIAuthID, userParams.APIToken))
      }, JsonRequestBehavior.AllowGet);

    }
    #endregion

  }
}
