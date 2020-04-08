using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CallCenter_BLL.Models.ViewModels;
using CallCenter_DAL;

namespace CallCenter_BLL.Controllers
{
  public class HomeController : Controller
  {
    private SaccoBLL _saccoBLL = new SaccoBLL();
    private MobileWithdrawalsBLL _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
    private BulkSMSBLL _bulkSMSBLL = new BulkSMSBLL();

    // GET: Home
    public ActionResult Index()
    {
      return View();
    }

    // GET: Home/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }

    // GET: Home/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: Home/Create
    [HttpPost]
    public ActionResult Create(FormCollection collection)
    {
      try
      {
        // TODO: Add insert logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }

    // GET: Home/Edit/5
    public ActionResult Edit(int id)
    {
      return View();
    }

    // POST: Home/Edit/5
    [HttpPost]
    public ActionResult Edit(int id, FormCollection collection)
    {
      try
      {
        // TODO: Add update logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }

    // GET: Home/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: Home/Delete/5
    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }
    #region Others
    [HttpGet]
    [Authorize]
    public ActionResult GetAllClientsFloats(string clientCorporateNo, bool loadAll)
    {

      /**
       * As at this point in time, calculation of sms floats
       * is on hold owing to the amount of time it takes to
       * query the archive db
       * 
       * There's a proposed approach for how to set the 
       * float running balance on every topup
       * 
       * This will be implemented at a later time.
       * Matthew Adote - 13Nov2019_1121
       **/

      // the flow:
      // get the float from the appropriate bll
      // compose the view model object

      List<SaccoFloatsViewModel> saccoFloatsVM = new List<SaccoFloatsViewModel>();
      List<Sacco> saccosList = new List<Sacco>();
      saccosList = _saccoBLL.GetSaccoList().ToList();
      List<MobileWithdrawals> saccoMobileWithdrawals = new List<MobileWithdrawals>();

      // get the float records for each sacco
      saccosList.Select(s => s.corporateNo).ToList().ForEach(s =>
      {
        MobileWithdrawals withdrawalRecord = _mobileWithdrawalsBLL.GetLatestWithdrawalForClient(s);
        if (withdrawalRecord == null) return;
        saccoMobileWithdrawals.Add(withdrawalRecord);
        // add the one for bulk sms here too

      });

      // prepare the data to return
      saccosList
        .Select(s => new { s.corporateNo, s.saccoName_1 })
        .ToList()
        .ForEach(s =>
        {
          MobileWithdrawals withdrawalRecord = saccoMobileWithdrawals
              .FirstOrDefault(w => w.Corporate_No == s.corporateNo);

          BulkSMSBalance saccoBulkSMSBalance = new BulkSMSBalance();
          try
          {
            saccoBulkSMSBalance = _bulkSMSBLL.GetBulkSMSBalanceForClient(s.corporateNo);
          }
          catch (InvalidOperationException)
          {
            saccoBulkSMSBalance = _bulkSMSBLL.GetEarliestBulkSMSBalanceRecordForClient(s.corporateNo);
          }

          saccoFloatsVM.Add(
            new SaccoFloatsViewModel
            {
              CorporateNo = s.corporateNo,
              SaccoName = s.saccoName_1,
              MPesaFloat = withdrawalRecord?.MPESA_Float_Amount,
              MpesaFloatDate = withdrawalRecord?.MPESA_DateTime,
              BulkSMSFloat = saccoBulkSMSBalance?.Balance,
              BulkSMSFloatDate = saccoBulkSMSBalance?.Last_Updated
            }
          );
        });
      return Json(saccoFloatsVM.ToArray(), JsonRequestBehavior.AllowGet);
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetMPesaFloat(string clientCorporateNo)
    {

      return null;
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetBulkSMSFloat(string clientCorporateNo)
    {

      return null;
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetLinkStatus(string clientCorporateNo)
    {

      //return this.RedirectToAction<LinkMonitoringController>(
      //  a => a.GetLinkStatusForClient(clientCorporateNo));
      return null;
    }
    #endregion
  }
}
