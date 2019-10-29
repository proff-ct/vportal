using System.Web.Mvc;
using CallCenter_DAL;

namespace CallCenter_BLL.Controllers
{
  public class SaccoController : Controller
  {
    private SaccoBLL _saccoBLL = new SaccoBLL();

    // GET: Index
    public ActionResult Index()
    {

      SetSaccoForView();
      return View(_saccoBLL.GetSaccoList());
    }

    [HttpPost]
    public string SetActiveSacco(int corporateNo)
    {
      Sacco activeSacco = _saccoBLL.GetSaccoByUniqueParam(corporateNo.ToString());
      Session["ActiveSacco"] = activeSacco;

      return activeSacco.saccoName_1;
    }
    public Sacco ActiveSacco
    {
      get
      {
        return (Sacco)Session["ActiveSacco"];
      }
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

  }
}
