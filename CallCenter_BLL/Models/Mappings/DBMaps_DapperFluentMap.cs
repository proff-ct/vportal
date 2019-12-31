using CallCenter_DAL;
using CallCenter_DAL.SMSFloat.Debit.Archived;
using CallCenter_DAL.SMSFloat.Debit.Unarchived;
using Dapper.FluentMap.Mapping;

namespace CallCenter_BLL.Models.Mappings
{
  internal class SaccoMap : EntityMap<Sacco>
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

  internal class MSaccoUtilityPaymentMap : EntityMap<MSaccoUtilityPayment>
  {
    internal MSaccoUtilityPaymentMap()
    {
      Map(s => s.Account_Balance).ToColumn("Account Balance");
      Map(s => s.Account_Name).ToColumn("Account Name");
      Map(s => s.Account_No).ToColumn("Account No");
      Map(s => s.Corporate_No).ToColumn("Corporate No");
      Map(s => s.Date_Created).ToColumn("Date Created");
      Map(s => s.Date_Sent_To_Journal).ToColumn("Date Sent To Journal");
      Map(s => s.Document_No).ToColumn("Document No");
      Map(s => s.Entry_No).ToColumn("Entry No");
      Map(s => s.Request_Confirmed).ToColumn("Request Confirmed");
      Map(s => s.Response_Status).ToColumn("Response Status");
      Map(s => s.Response_Value).ToColumn("Response Value");
      Map(s => s.Sent_To_Journal).ToColumn("Sent To Journal");
      Map(s => s.SESSION_ID).ToColumn("SESSION_ID");
      Map(s => s.Telephone_No).ToColumn("Telephone No");
      Map(s => s.Time_Created).ToColumn("Time Created");
      Map(s => s.Time_Sent_To_Journal).ToColumn("Time Sent To Journal");
      Map(s => s.Transaction_Date).ToColumn("Transaction Date");
      Map(s => s.Utility_Float_Balance).ToColumn("Utility Float Balance");
      Map(s => s.Utility_Payment_Account).ToColumn("Utility Payment Account");
      Map(s => s.Utility_Payment_Type).ToColumn("Utility Payment Type");
      Map(s => s.Utility_Received).ToColumn("Utility Received");
      Map(s => s.Utility_Result_Type).ToColumn("Utility Result Type");
    }
  }
  internal class MSaccoAirtimeTopupMap : EntityMap<MSaccoAirtimeTopup>
  {
    internal MSaccoAirtimeTopupMap()
    {
      Map(m => m.Account_Balance).ToColumn("Account Balance");
      Map(m => m.Account_Name).ToColumn("Account Name");
      Map(m => m.Account_No).ToColumn("Account No");
      Map(m => m.Airtime_Float_Balance).ToColumn("Airtime Float Balance");
      Map(m => m.Airtime_Received).ToColumn("Airtime Received");
      Map(m => m.Airtime_Result_Type).ToColumn("Airtime Result Type");
      Map(m => m.Corporate_No).ToColumn("Corporate No");
      Map(m => m.Date_Created).ToColumn("Date Created");
      Map(m => m.Date_Sent_To_Journal).ToColumn("Date Sent To Journal");
      Map(m => m.Document_No).ToColumn("Document No");
      Map(m => m.Entry_No).ToColumn("Entry No");
      Map(m => m.From_MSISDN).ToColumn("From MSISDN");
      Map(m => m.Response_Status).ToColumn("Response Status");
      Map(m => m.Response_Value).ToColumn("Response Value");
      Map(m => m.Sent_To_Journal).ToColumn("Sent To Journal");
      Map(m => m.Time_Created).ToColumn("Time Created");
      Map(m => m.Time_Sent_To_Journal).ToColumn("Time Sent To Journal");
      Map(m => m.Transaction_Date).ToColumn("Transaction Date");
    }
  }
  internal class MobileWithdrawalsMap : EntityMap<MobileWithdrawals>
  {
    internal MobileWithdrawalsMap()
    {
      Map(m => m.Account_Balance).ToColumn("Account Balance");
      Map(m => m.Account_Name).ToColumn("Account Name");
      Map(m => m.Account_No).ToColumn("Account No");
      Map(m => m.Broker_Conv_ID).ToColumn("Broker Conv ID");
      Map(m => m.Broker_Orig_Conv_ID).ToColumn("Broker Orig Conv ID");
      Map(m => m.Broker_Resp_Code).ToColumn("Broker Resp Code");
      Map(m => m.Broker_Resp_Desc).ToColumn("Broker Resp Desc");
      Map(m => m.Broker_Service_Status).ToColumn("Broker Service Status");
      Map(m => m.Corporate_No).ToColumn("Corporate No");
      Map(m => m.Date_Created).ToColumn("Date Created");
      Map(m => m.Date_Sent_To_Journal).ToColumn("Date Sent To Journal");
      Map(m => m.Date_SMS_Sent).ToColumn("Date SMS Sent");
      Map(m => m.Document_No).ToColumn("Document No");
      Map(m => m.Exported_To_Saf).ToColumn("Exported To Saf");
      Map(m => m.Entry_No).ToColumn("Entry No");
      Map(m => m.Money_Received).ToColumn("Money Received");
      Map(m => m.MPESA_Conv_ID).ToColumn("MPESA Conv ID");
      Map(m => m.MPESA_DateTime).ToColumn("MPESA DateTime");
      Map(m => m.MPESA_Float_Amount).ToColumn("MPESA Float Amount");
      Map(m => m.MPesa_Name).ToColumn("MPesa Name");
      Map(m => m.MPESA_Receipt_No).ToColumn("MPESA Receipt No");
      Map(m => m.MPESA_Result_Code).ToColumn("MPESA Result Code");
      Map(m => m.MPESA_Result_Desc).ToColumn("MPESA Result Desc");
      Map(m => m.MPESA_Result_Type).ToColumn("MPESA Result Type");
      Map(m => m.Request_Confirmed).ToColumn("Request Confirmed");
      Map(m => m.Response_Status).ToColumn("Response Status");
      Map(m => m.Response_Value).ToColumn("Response Value");
      Map(m => m.Sent_To_Journal).ToColumn("Sent To Journal");
      Map(m => m.SMS_Sent).ToColumn("SMS Sent");
      Map(m => m.Telephone_No).ToColumn("Telephone No");
      Map(m => m.Text_File_Created).ToColumn("Text File Created");
      Map(m => m.Time_Created).ToColumn("Time Created");
      Map(m => m.Time_Sent_To_Journal).ToColumn("Time Sent To Journal");
      Map(m => m.Time_SMS_Sent).ToColumn("Time SMS Sent");
      Map(m => m.Transaction_Date).ToColumn("Transaction Date");
    }
  }
  internal class LinkMonitoringMap : EntityMap<LinkMonitoring>
  {
    internal LinkMonitoringMap()
    {
      Map(m => m.Corporate_Name).ToColumn("Corporate Name");
      Map(m => m.Corporate_No).ToColumn("Corporate No");
      Map(m => m.Entry_No).ToColumn("Entry No");
      Map(m => m.Http_Status).ToColumn("Http Status");
      Map(m => m.Last_Check).ToColumn("Last Check");
      Map(m => m.Last_Email_Sent).ToColumn("Last Email Sent");
      Map(m => m.Ping_Result).ToColumn("Ping Result");
    }
  }
  internal class UnarchivedBulkSMSDebitMap : EntityMap<UnarchivedBulkSMSDebit>
  {
    internal UnarchivedBulkSMSDebitMap()
    {
      Map(m => m.Back_Up_SMS).ToColumn("Back Up SMS");
      Map(m => m.Batch_No).ToColumn("Batch No");
      Map(m => m.Bulk_SMS).ToColumn("Bulk SMS");
      Map(m => m.Bulk_SMS_Date).ToColumn("Bulk SMS Date");
      Map(m => m.Bulk_SMS_ID).ToColumn("Bulk SMS ID");
      Map(m => m.Bulk_SMS_No).ToColumn("Bulk SMS No");
      Map(m => m.Bulk_SMS_Time).ToColumn("Bulk SMS Time");
      Map(m => m.Corporate_No).ToColumn("Corporate No");
      Map(m => m.Date_Payment_Processed).ToColumn("Date Payment Processed");
      Map(m => m.EMail_Sent).ToColumn("E-Mail Sent");
      Map(m => m.From_Portal).ToColumn("From Portal");
      Map(m => m.Marked_For_Email).ToColumn("Marked For E-mail");
      Map(m => m.Message_Type_ID).ToColumn("Message Type ID");
      Map(m => m.Payment_Processed).ToColumn("Payment Processed");
      Map(m => m.Processed_By).ToColumn("Processed By");
      Map(m => m.Reply_Sent).ToColumn("Reply Sent");
      Map(m => m.SMS_Date).ToColumn("SMS Date");
      Map(m => m.SMS_Sent).ToColumn("SMS Sent");
      Map(m => m.Time_Payment_Processed).ToColumn("Time Payment Processed");
    }
  }
  internal class ArchivedBulkSMSDebitMap : EntityMap<ArchivedBulkSMSDebit>
  {
    internal ArchivedBulkSMSDebitMap()
    {
      Map(m => m.Back_Up_SMS).ToColumn("Back Up SMS");
      Map(m => m.Bulk_SMS).ToColumn("Bulk SMS");
      Map(m => m.Bulk_SMS_Date).ToColumn("Bulk SMS Date");
      Map(m => m.Bulk_SMS_ID).ToColumn("Bulk SMS ID");
      Map(m => m.Bulk_SMS_No).ToColumn("Bulk SMS No");
      Map(m => m.Bulk_SMS_Time).ToColumn("Bulk SMS Time");
      Map(m => m.Corporate_No).ToColumn("Corporate No");
      Map(m => m.Date_Payment_Processed).ToColumn("Date Payment Processed");
      Map(m => m.EMail_Sent).ToColumn("E-Mail Sent");
      Map(m => m.Marked_For_Email).ToColumn("Marked For E-mail");
      Map(m => m.Message_Type_ID).ToColumn("Message Type ID");
      Map(m => m.Payment_Processed).ToColumn("Payment Processed");
      Map(m => m.Processed_By).ToColumn("Processed By");
      Map(m => m.Reply_Sent).ToColumn("Reply Sent");
      Map(m => m.SMS_Date).ToColumn("SMS Date");
      Map(m => m.SMS_Sent).ToColumn("SMS Sent");
      Map(m => m.Time_Payment_Processed).ToColumn("Time Payment Processed");
    }
  }

  internal class LinkDowntimeMap : EntityMap<LinkDowntime>
  {
    internal LinkDowntimeMap()
    {
      Map(m => m.Corporate_Name).ToColumn("Corporate Name");
      Map(m => m.Corporate_No).ToColumn("Corporate No");
      Map(m => m.Entry_No).ToColumn("Entry No");
      Map(m => m.Downtime_Duration).ToColumn("Downtime Duration");
      Map(m => m.Downtime_Start).ToColumn("Downtime Start");
      Map(m => m.Downtime_Stop).ToColumn("Downtime Stop");
      Map(m => m.Last_Check).ToColumn("Last Check");
    }
  }
  internal class BulkSMSBalanceMap : EntityMap<BulkSMSBalance>
  {
    internal BulkSMSBalanceMap()
    {
      Map(m => m.Corporate_No).ToColumn("Corporate No");
      Map(m => m.Entry_No).ToColumn("Entry No");
      Map(m => m.Last_Updated).ToColumn("Last Updated");
      
    }
  }

}
     