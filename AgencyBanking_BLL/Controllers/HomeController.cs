using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgencyBanking_BLL.Controllers
{[Authorize]
    public class HomeController : Controller
  {
      
        public ActionResult Index()
    {
        SummaryBLL Summarybll = new SummaryBLL();

            Dictionary<string, string> data = Summarybll.GetSummary();
        ViewBag.Data = data;
        return View();
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }
    [Authorize]
    public ContentResult GetDeposits()
    {
        SummaryBLL Summarybll = new SummaryBLL();
        return Content(Summarybll.GetDeposit_vs_sharesDeposit(), "application/json");
    }
    [Authorize]
    public ContentResult GetLoanStats()
    {
        SummaryBLL Summarybll = new SummaryBLL();
        return Content(Summarybll.Loan_Application_Count(), "application/json");
    } 
    [Authorize]
    public ContentResult GetMemberRegistrationStats()
    {
        SummaryBLL Summarybll = new SummaryBLL();
        return Content(Summarybll.Member_Registration_Count(), "application/json");
    }
    [Authorize]
    public ContentResult GetLoanRepaymentStats()
    {
        SummaryBLL Summarybll = new SummaryBLL();
        return Content(Summarybll.Loan_Repayment_Stats(), "application/json");
    }

  }
}