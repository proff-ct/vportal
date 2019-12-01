using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using VisibilityPortal_BLL;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ViewModels;
using VisibilityPortal_BLL.Utilities.MSSQLOperators;
using VisibilityPortal_DAL;

namespace Visibility_Portal.Controllers
{
  public class UserController : Controller
  {
    private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
    private PortalModuleBLL _portalModuleBLL = new PortalModuleBLL();

    // GET: CoretecClient
    public ActionResult Index()
    {
      return View();
    }

    // GET: CoretecClient/Edit/5
    public ActionResult AddOrUpdate(string clientModuleId = "")
    {

      return null;
    }
    // POST: CoretecClient/Edit/5
    [HttpPost]
    public ActionResult AddOrUpdate(AddPortalModuleForClientViewModel registerClientModule = null, CoretecClientModuleViewModel updateClientModule = null)
    {
     
      return RedirectToAction("Index");
    }


    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetRegisteredClients(int page, int size)
    {

      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function


      //PaginationParameters pagingParams = new PaginationParameters(
      //  int.Parse(page), int.Parse(size), null);
      PaginationParameters pagingParams = new PaginationParameters(page, size, null);

      dynamic registeredClientsRecords = _coretecClientBLL
        .GetListOfClientsWithModules(out int lastPage, true, pagingParams)
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = registeredClientsRecords
      }, JsonRequestBehavior.AllowGet);

    }
    [HttpGet]
    [Authorize]
    public ActionResult GetRegisteredClientModules(int page, int size)
    {

      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function


      //PaginationParameters pagingParams = new PaginationParameters(
      //  int.Parse(page), int.Parse(size), null);
      PaginationParameters pagingParams = new PaginationParameters(page, size, null);

      dynamic registeredClientsRecords = _coretecClientBLL
        .GetListOfClientModules(out int lastPage, true, pagingParams)
        .ToArray();

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = registeredClientsRecords
      }, JsonRequestBehavior.AllowGet);

    }
    [HttpGet]
    [Authorize]
    public ActionResult GetUnregisteredClients(string mode = "forTable")
    {
      IEnumerable<CoreTecClient> clientList = _coretecClientBLL.GetUnregisteredClients();
      switch (mode)
      {
        case "forSelect":
          return Json(clientList.OrderBy(c => c.saccoName_1).Select(s => new SelectListItem()
          {
            Text = s.saccoName_1,
            Value = s.corporateNo
          }).ToList(), JsonRequestBehavior.AllowGet);

        default:
          return null;
      }

    }
    #endregion


  }
}
