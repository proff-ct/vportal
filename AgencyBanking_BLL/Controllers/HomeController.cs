using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using AgencyBanking_BLL.util;
using AgencyBanking_DAL;
using Newtonsoft.Json;
using Utilities.PortalApplicationParams;

namespace AgencyBanking_BLL.Controllers
{
    [Authorize]
    [CorporateNumberFilter]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            SummaryBLL Summarybll = new SummaryBLL();
            Dictionary<string, string> data = Summarybll.GetSummary(CurrentSacco.CorporateNo);
            data.Remove("Transaction Type");
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

            return Content(Summarybll.GetDeposit_vs_sharesDeposit(CurrentSacco.CorporateNo), "application/json");
        }
        [Authorize]
        public ContentResult GetLoanStats()
        {

            SummaryBLL Summarybll = new SummaryBLL();

            //Are we dealing with GFL Sacco?
            if (CurrentSacco.CorporateNo == "CAP016")
            {
                string query =
                    "SELECT isnull(count(*),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',CONCAT (DATeName(mm,[transaction date])+' ',DATEPART(year,[transaction date])) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].PUTransactions where ([Transaction Type] = 'Loan Application') and [Transaction Date] >=  Dateadd(Month, Datediff(Month, 0, DATEADD(m, -12,current_timestamp)), 0)  group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto;\r\n";
                return Content(Summarybll.RunQuery(query), "application/json");
            }
            return Content(Summarybll.Loan_Application_Count(CurrentSacco.CorporateNo), "application/json");
        }
        [Authorize]
        public ContentResult GetMemberRegistrationStats()
        {
            SummaryBLL Summarybll = new SummaryBLL();
            if (CurrentSacco.CorporateNo == "CAP016")
            {
                string query =
                    "SELECT isnull(count(*),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',CONCAT (DATeName(mm,[transaction date])+' ',DATEPART(year,[transaction date])) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].PUTransactions where ([Transaction Type] = 'Member Registration') and [Transaction Date] >=  Dateadd(Month, Datediff(Month, 0, DATEADD(m, -12,current_timestamp)), 0)  group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date])\r\n for json auto";
                return Content(Summarybll.RunQuery(query), "application/json");
            }
            return Content(Summarybll.Member_Registration_Count(CurrentSacco.CorporateNo), "application/json");
        }
        [Authorize]
        public ContentResult GetLoanRepaymentStats()
        {
            SummaryBLL Summarybll = new SummaryBLL();
            //Returns The Mpesa paybill counts
            if (CurrentSacco.CorporateNo == "CAP016")
            {
                string query =
                    "SELECT isnull(count(*),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',CONCAT (DATeName(mm,[transaction date])+' ',DATEPART(year,[transaction date])) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].PUTransactions where ([Transaction Type] = 'Mpesa paybill') and [Transaction Date] >=  Dateadd(Month, Datediff(Month, 0, DATEADD(m, -12,current_timestamp)), 0)  group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto\r\n";                return Content(Summarybll.RunQuery(query), "application/json");
            }

            return Content(Summarybll.Loan_Repayment_Stats(CurrentSacco.CorporateNo), "application/json");
        }

        #region ForTestPurposes
        private string SetHeaders<T>(T t)
        {
            PropertyInfo[] Properties = GetProperties(t);
            List<string> gflColumnHeaders = new List<string>
      {
        "Entry_No",
        "Account_No",
        "Transaction_By",
        "Transaction_Date",
        "Transaction_Type",
        "Agent_Code",
        "Comments",
        "DeviceID"
      };
            List<TableHeader> list = new List<TableHeader>();
            foreach (PropertyInfo pro in Properties)
            {
                if (!gflColumnHeaders.Contains(pro.Name)) continue;
                list.Add(new TableHeader()
                {
                    field = pro.Name,
                    title = pro.Name,
                    sorter = "string",
                    headerFilter = "input"
                });
            }

            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// Using Reflection to Get Properties
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties();
        }
        #endregion
    }
}