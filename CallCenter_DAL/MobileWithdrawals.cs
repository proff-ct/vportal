using CallCenter_Dataspecs.Models;
using System;

namespace CallCenter_DAL
{
  public class MobileWithdrawals : IDBModel, IMobileWithdrawals_SACCODB
  {
    public int Entry_No { get; set; }

    public string Status { get; set; }

    public string Account_No { get; set; }

    public string Account_Name { get; set; }

    public string Telephone_No { get; set; }

    public decimal? Amount { get; set; }

    public string Comments { get; set; }

    public string Text_File_Created { get; set; }

    public DateTime? Date_Created { get; set; }

    public DateTime? Time_Created { get; set; }

    public string SMS_Sent { get; set; }

    public DateTime? Date_SMS_Sent { get; set; }

    public DateTime? Time_SMS_Sent { get; set; }

    public string SESSION_ID { get; set; }

    public DateTime? Transaction_Date { get; set; }

    public string Sent_To_Journal { get; set; }

    public DateTime? Date_Sent_To_Journal { get; set; }

    public DateTime? Time_Sent_To_Journal { get; set; }

    public string Corporate_No { get; set; }

    public decimal? Account_Balance { get; set; }

    public string Request_Confirmed { get; set; }

    public string Response_Status { get; set; }

    public string Response_Value { get; set; }

    public DateTime? Datetime { get; set; }

    public int? Money_Received { get; set; }

    public bool? Source { get; set; }

    public int? TransactionType { get; set; }

    public int? Saccosource { get; set; }

    public string Broker_Resp_Code { get; set; }

    public string Broker_Resp_Desc { get; set; }

    public string Broker_Conv_ID { get; set; }

    public string Broker_Orig_Conv_ID { get; set; }

    public string Broker_Service_Status { get; set; }

    public string MPESA_Result_Type { get; set; }

    public string MPESA_Result_Code { get; set; }

    public string MPESA_Result_Desc { get; set; }

    public string MPESA_Conv_ID { get; set; }

    public string MPESA_Receipt_No { get; set; }

    public DateTime? MPESA_DateTime { get; set; }

    public decimal? MPESA_Float_Amount { get; set; }

    public string Remarks { get; set; }

    public string Document_No { get; set; }

    public string MPesa_Name { get; set; }

    public int IsDirectDisbursement { get; set; }

    public string Exported_To_Saf { get; set; }

    public bool? Verified { get; set; }

    public static string DBName => "SACCO";
    public static string DBTableName => "[Mobile Withdrawals]";
    public string databaseName => DBName;

    public string tableName => DBTableName;
  }

  public class MobileWithdrawals_Daraja : IMobileWithdrawals_DarajaDB, IDBModel
  {
    public string databaseName => DarajaDB_MobileWithdrawals.DBName;

    public string tableName => DarajaDB_MobileWithdrawals.TableName;
    public int Entry_No { get; set; }
    public string Status { get; set; }
    public string Account_No { get; set; }
    public string Account_Name { get; set; }
    public string Telephone_No { get; set; }
    public decimal? Amount { get; set; }
    public string Comments { get; set; }
    public string Text_File_Created { get; set; }
    public DateTime? Date_Created { get; set; }
    public DateTime? Time_Created { get; set; }
    public string SMS_Sent { get; set; }
    public DateTime? Date_SMS_Sent { get; set; }
    public DateTime? Time_SMS_Sent { get; set; }
    public string SESSION_ID { get; set; }
    public DateTime? Transaction_Date { get; set; }
    public string Sent_To_Journal { get; set; }
    public DateTime? Date_Sent_To_Journal { get; set; }
    public DateTime? Time_Sent_To_Journal { get; set; }
    public string Corporate_No { get; set; }
    public decimal? Account_Balance { get; set; }
    public string Request_Confirmed { get; set; }
    public string Response_Status { get; set; }
    public string Response_Value { get; set; }
    public DateTime? Datetime { get; set; }
    public int? Money_Received { get; set; }
    public bool? Source { get; set; }
    public int? TransactionType { get; set; }
    public int? Saccosource { get; set; }
    public string Broker_Resp_Code { get; set; }
    public string Broker_Resp_Desc { get; set; }
    public string Broker_Conv_ID { get; set; }
    public string Broker_Orig_Conv_ID { get; set; }
    public string Broker_Service_Status { get; set; }
    public string MPESA_Result_Type { get; set; }
    public string MPESA_Result_Code { get; set; }
    public string MPESA_Result_Desc { get; set; }
    public string MPESA_Conv_ID { get; set; }
    public string MPESA_Receipt_No { get; set; }
    public DateTime? MPESA_DateTime { get; set; }
    public decimal? MPESA_Float_Amount { get; set; }
    public string Remarks { get; set; }
    public string Document_No { get; set; }
    public string MPesa_Name { get; set; }
    public int IsDirectDisbursement { get; set; }
  }
}
