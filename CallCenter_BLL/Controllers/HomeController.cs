using System.Web.Mvc;

namespace CallCenter_BLL.Controllers
{
  public class HomeController : Controller
  {
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
    public ActionResult GetMPesaFloat(string clientCorporateNo) {

      return null;
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetBulkSMSFloat(string clientCorporateNo) {

      return null;
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetTrxSMSFloat(string clientCorporateNo) {

      return null;
    }
    [HttpGet]
    [Authorize]
    public ActionResult GetLinkStatus(string clientCorporateNo) {

      //return this.RedirectToAction<LinkMonitoringController>(
      //  a => a.GetLinkStatusForClient(clientCorporateNo));
      return null;
    }
    #endregion
  }
}
