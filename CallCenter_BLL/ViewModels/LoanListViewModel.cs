using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_BLL.ViewModels
{
  public class LoanListViewModel
  {
    public int Entry_No { get; set; }
    public string Status { get; set; }
    public string Account_No { get; set; }
    public string Telephone_No { get; set; }
    public decimal? Amount { get; set; }
    public string SESSION_ID { get; set; }
    public DateTime? Transaction_Date { get; set; }
    public string Sent_To_Journal { get; set; }
    public string Corporate_No { get; set; }
    public string Type { get; set; }
    public string Loan_type { get; set; }
    public int? No_of_Guarantors { get; set; }
    public bool? Disbursed { get; set; }
  }

}
