using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_BLL.ViewModels;
using CallCenter_DAL;

namespace CallCenter_BLL.Controllers
{
  public class FloatAlertsController : Controller
  {
    private SaccoBLL _saccoBLL = new SaccoBLL();
    private AlertTypeBLL _alertTypeBLL = new AlertTypeBLL();
    private FloatResourceBLL _floatResourceBLL = new FloatResourceBLL();
    private FloatResourceAlertForClientBLL _clientFRABLL = new FloatResourceAlertForClientBLL();

    // GET: FloatAlerts
    public ActionResult Index()
    {
      return View();
    }

    // GET: FloatAlerts/Edit/5
    public ActionResult AddOrUpdate(int clientFloatAlertId = 0)
    {
      SetFloatAlertViewParams();

      if (clientFloatAlertId.Equals(0))
      {
        return View();
      }
      else
      {
        SaccoFloatAlertViewModel model = Mapper
          .Map<SaccoFloatAlertViewModel>(
          _clientFRABLL.GetFloatResourceAlertForClientById(clientFloatAlertId));

        ViewBag.AlertTypeList = Mapper
        .Map<List<AlertTypeViewModel>>(_alertTypeBLL.GetAllAlertTypes().ToList())
        .Where(alertType => alertType.Id.Equals(int.Parse(model.AlertTypeId)))
        .ToList();

        return View(model);
      }

    }
    // POST: FloatAlerts/Edit/5
    [HttpPost]
    public ActionResult AddOrUpdate(SaccoFloatAlertViewModel model)
    {

      // check the model state
      if (!ModelState.IsValid)
      {
        SetFloatAlertViewParams(model.ClientCorporateNo);

        return View(model);
      }

      // Proceed with saving to db
      bool isNew = false;
      if (model.Id.Equals(0))
      {
        model.CreatedBy = User.Identity.Name;
        isNew = true;
      }
      else
      {
        model.ModifiedBy = User.Identity.Name;
      }
      
      if (!_clientFRABLL.Save(
          Mapper.Map<FloatResourceAlertForClient>(model),
          isNew ? ModelOperation.AddNew : ModelOperation.Update))
      {
        SetFloatAlertViewParams(model.ClientCorporateNo);
        ModelState.AddModelError("", "A problem occurred while saving to db. Please try again");
        return View(model);
      }
      else
      {
        return RedirectToAction("Index");

      }
    }
    #region PrivateOnes
    private void SetFloatAlertViewParams(string corporateNo = null)
    {
      ViewBag.SaccoList = Mapper.Map<List<SaccoViewModel>>(_saccoBLL.GetSaccoList().OrderBy(s=>s.saccoName_1).ToList());

      ViewBag.FloatResourceList = Mapper
        .Map<List<FloatResourceViewModel>>(_floatResourceBLL.GetFloatResourcesList().OrderBy(r=>r.ResourceName).ToList());
    }

    #endregion

    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetAllFloatResourceAlerts(int page, int size, string clientCorporateNo = null)
    {

      // the flow:
      // 1. get the pagination parameters
      // 2. pass the pagination parameters to the bll function
      // 3. retrieve the data from the bll function
      PaginationParameters pagingParams = new PaginationParameters(page, size, null);
      int lastPage = 0;
      dynamic floatResourceAlertRecords = null;

      if (!string.IsNullOrEmpty(clientCorporateNo))
      {
        floatResourceAlertRecords = Mapper
        .Map<IEnumerable<SaccoFloatAlertListViewModel>>(
        _clientFRABLL.GetListOfFloatResourceAlertsForClient(clientCorporateNo))
        .Select(m =>
        {
          m.SaccoName = _saccoBLL.GetSaccoByUniqueParam(m.ClientCorporateNo).saccoName_1;
          m.AlertType = _alertTypeBLL.GetAlertTypeByName(
            Enum.GetName(typeof(AlertType.AlertTypes), int.Parse(m.AlertTypeId) - 1)).AlertName;
          m.FloatResource = _floatResourceBLL.GetFloatResourceById(m.FloatResourceId).ResourceName;

          return m;
        })
        .ToArray();
      }
      else
      {
        floatResourceAlertRecords = Mapper
        .Map<IEnumerable<SaccoFloatAlertListViewModel>>(
        _clientFRABLL.GetListOfFloatResourceAlertsForAllClients(out lastPage, true, pagingParams))
        .Select(m =>
        {
          m.SaccoName = _saccoBLL.GetSaccoByUniqueParam(m.ClientCorporateNo).saccoName_1;
          m.AlertType = _alertTypeBLL.GetAlertTypeByName(
            Enum.GetName(typeof(AlertType.AlertTypes), int.Parse(m.AlertTypeId) - 1)).AlertName;
          m.FloatResource = _floatResourceBLL.GetFloatResourceById(m.FloatResourceId).ResourceName;

          return m;
        })
        .ToArray();
      }

      return Json(new
      {
        last_page = lastPage, // last page from the fetched recordset
        data = floatResourceAlertRecords
      }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [Authorize]
    public ActionResult GetConfigurableAlertsForClientResource(string clientCorporateNo, string floatResourceId)
    {
      if (string.IsNullOrEmpty(clientCorporateNo) && string.IsNullOrEmpty(floatResourceId))
      {
        return null;
      }

      List<FloatResourceAlertForClient> clientFRAList = _clientFRABLL
        .GetListOfFloatResourceAlertsForClient(clientCorporateNo).ToList();

      // exclude alert levels which have already been configured
      List<AlertTypeViewModel> alertTypeList = Mapper
        .Map<List<AlertTypeViewModel>>(_alertTypeBLL.GetAllAlertTypes().ToList())
        .Where(alertType => !clientFRAList.Exists(clientFRAlert =>
          clientFRAlert.FloatResourceId.Equals(floatResourceId) &&
          clientFRAlert.AlertTypeId.Equals(alertType.Id) 
        ))
        .ToList();

      return Json(alertTypeList.OrderBy(s => s.AlertName).Select(s => new SelectListItem()
      {
        Text = s.AlertName,
        Value = s.Id.ToString()
      }).ToList(), JsonRequestBehavior.AllowGet);
    }
    #endregion


  }
}
