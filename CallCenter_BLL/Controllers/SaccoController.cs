using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CallCenter_DAL;

namespace CallCenter_BLL.Controllers
{
  public class SaccoController : Controller
  {
    private SaccoBLL _saccoBLL = new SaccoBLL();
    public Sacco ActiveSacco => (Sacco)Session["ActiveSacco"];

    // GET: Index
    public ActionResult Index()
    {

      SetSaccoForView();
      return View(_saccoBLL.GetSaccoList());
    }
    [HttpGet]
    public ActionResult LoadPrimaryInfo()
    {
      // Primary info is:
      //M-Pesa float, Bulk SMS float, Transactional SMS float
      return null;
    }

    [HttpPost]
    public string SetActiveSacco(int corporateNo)
    {
      Sacco activeSacco = _saccoBLL.GetSaccoByUniqueParam(corporateNo.ToString());
      Session["ActiveSacco"] = activeSacco;

      return activeSacco.saccoName_1;
    }
    
    public void SetSaccoForView()
    {
      ViewBag.Sacco = ActiveSacco;
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

    [HttpGet]
    [Authorize]
    public ActionResult GetSaccoList(string mode="forTable")
    {

      IEnumerable<Sacco> saccoList = _saccoBLL.GetSaccoList();
      switch (mode)
      {
        case "forSelect":
          return Json(saccoList.OrderBy(s => s.saccoName_1).Select(s => new SelectListItem()
          {
            Text = s.saccoName_1,
            Value = s.corporateNo
          }).ToList(), JsonRequestBehavior.AllowGet);

        default:
          return null;
      }
    }

  }
}
