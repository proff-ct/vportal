using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_DAL;
using Dapper.FluentMap.Mapping;

namespace CallCenter_BLL.Models.Mappings
{
  class SaccoMap : EntityMap<Sacco>
  {
    internal SaccoMap()
    {
      Map(s => s.corporateNo).ToColumn("Corporate No");
      Map(s => s.corporateNo_2).ToColumn("Corporate No 2");
      Map(s => s.saccoName_1).ToColumn("Sacco Name 1");
      Map(s => s.mpesaFloat).ToColumn("Mpesa Float");
    }
  }

  internal class MSaccoSalaryAdvanceBaseMap<T> : EntityMap<MSaccoSalaryAdvance>
  {
    internal MSaccoSalaryAdvanceBaseMap()
    {
      Map(r => r.Account_Balance).ToColumn("Account Balance");
      Map(r => r.Account_No).ToColumn("Account No");
      Map(r => r.Bonus_based).ToColumn("Bonus based");
      Map(r => r.Client_Code).ToColumn("Client Code");
      Map(r => r.Client_Name).ToColumn("Client Name");
      Map(r => r.Corporate_No).ToColumn("Corporate No");
      Map(r => r.Date_Sent_To_Journal).ToColumn("Date Sent To Journal");
      Map(r => r.Entry_No).ToColumn("Entry No");
      Map(r => r.G_Appraised).ToColumn("G Appraised");
      Map(r => r.KG_Guaranteed).ToColumn("KG Guaranteed");
      Map(r => r.KG_Used).ToColumn("KG Used");
      Map(r => r.Loan_Name).ToColumn("Loan Name");
      Map(r => r.Loan_Status).ToColumn("Loan Status");
      Map(r => r.Loan_type).ToColumn("Loan type");
      Map(r => r.Max_Loan).ToColumn("Max Loan");
      Map(r => r.Member_No).ToColumn("Member No");
      Map(r => r.Min_Loan).ToColumn("Min Loan");
      Map(r => r.Mounthly_Installments).ToColumn("Mounthly Installments");
      Map(r => r.Net_Pay).ToColumn("Net Pay");
      Map(r => r.No_of_Guarantors).ToColumn("No of Guarantors");
      Map(r => r.Processing_fee).ToColumn("Processing fee");
      Map(r => r.Repayment_Period).ToColumn("Repayment Period");
      Map(r => r.Sent_To_Journal).ToColumn("Sent To Journal");
      Map(r => r.SESSION_ID).ToColumn("SESSION_ID");
      Map(r => r.Staff_No).ToColumn("Staff No");
      Map(r => r.Telephone_No).ToColumn("Telephone No");
      Map(r => r.Time_Sent_To_Journal).ToColumn("Time Sent To Journal");
      Map(r => r.Transaction_Date).ToColumn("Transaction Date");
    }
  }
  internal class MSaccoSalaryAdvanceMap : MSaccoSalaryAdvanceBaseMap<MSaccoSalaryAdvance>
  {
    internal MSaccoSalaryAdvanceMap() : base() { }
  }
  internal class GFLLoanMap : MSaccoSalaryAdvanceBaseMap<GFLLoan>
  {
    internal GFLLoanMap() : base() { }
  }
  internal class GuarantorsMap : EntityMap<Guarantors>
  {
    internal GuarantorsMap()
    {
      Map(r => r.Loan_Type).ToColumn("Loan Type");
      Map(r => r.G_Account).ToColumn("G_Account");
      Map(r => r.G_Account_Name).ToColumn("G_Account_Name");
      Map(r => r.Amount_Guaranteed).ToColumn("Amount Guaranteed");
      Map(r => r.G_Phone_No).ToColumn("G Phone No");
      Map(r => r.Sent_Sms).ToColumn("Sent Sms");
      Map(r => r.Guarantor_Session).ToColumn("Guarantor Session");
      Map(r => r.Date_Responded).ToColumn("Date Responded");
      Map(r => r.Date_sms_sent).ToColumn("Date sms sent");
      Map(r => r.Updated_To_Client).ToColumn("Updated To Client");
    }
  }

}
