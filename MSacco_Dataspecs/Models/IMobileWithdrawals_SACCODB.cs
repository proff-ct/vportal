using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Models
{
  public interface IMobileWithdrawals_SACCODB
  {
    decimal? Account_Balance { get; set; }
    string Account_Name { get; set; }
    string Account_No { get; set; }
    decimal? Amount { get; set; }
    string Broker_Conv_ID { get; set; }
    string Broker_Orig_Conv_ID { get; set; }
    string Broker_Resp_Code { get; set; }
    string Broker_Resp_Desc { get; set; }
    string Broker_Service_Status { get; set; }
    string Comments { get; set; }
    string Corporate_No { get; set; }
    DateTime? Date_Created { get; set; }
    DateTime? Date_Sent_To_Journal { get; set; }
    DateTime? Date_SMS_Sent { get; set; }
    DateTime? Datetime { get; set; }
    string Document_No { get; set; }
    int Entry_No { get; set; }
    string Exported_To_Saf { get; set; }
    int IsDirectDisbursement { get; set; }
    int? Money_Received { get; set; }
    string MPESA_Conv_ID { get; set; }
    DateTime? MPESA_DateTime { get; set; }
    decimal? MPESA_Float_Amount { get; set; }
    string MPesa_Name { get; set; }
    string MPESA_Receipt_No { get; set; }
    string MPESA_Result_Code { get; set; }
    string MPESA_Result_Desc { get; set; }
    string MPESA_Result_Type { get; set; }
    string Remarks { get; set; }
    string Request_Confirmed { get; set; }
    string Response_Status { get; set; }
    string Response_Value { get; set; }
    int? Saccosource { get; set; }
    string Sent_To_Journal { get; set; }
    string SESSION_ID { get; set; }
    string SMS_Sent { get; set; }
    bool? Source { get; set; }
    string Status { get; set; }
    string Telephone_No { get; set; }
    string Text_File_Created { get; set; }
    DateTime? Time_Created { get; set; }
    DateTime? Time_Sent_To_Journal { get; set; }
    DateTime? Time_SMS_Sent { get; set; }
    DateTime? Transaction_Date { get; set; }
    int? TransactionType { get; set; }
    bool? Verified { get; set; }
  }
}
