using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL
{
  public class MSaccoAirtimeTopup : IDBModel
  {
    public int Entry_No { get; set; }

    public string SESSION_ID { get; set; }

    public string Status { get; set; }

    public string Account_No { get; set; }

    public string Account_Name { get; set; }

    public string TelephoneNo { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date_Created { get; set; }

    public DateTime? Time_Created { get; set; }

    public DateTime? Transaction_Date { get; set; }

    public decimal? Account_Balance { get; set; }

    public bool? RequestConfirmed { get; set; }

    public bool? Sent_To_Journal { get; set; }

    public DateTime? Date_Sent_To_Journal { get; set; }

    public DateTime? Time_Sent_To_Journal { get; set; }

    public string Corporate_No { get; set; }

    public string Comments { get; set; }

    public string Response_Status { get; set; }

    public string Response_Value { get; set; }

    public DateTime? Datetime { get; set; }

    public bool? Source { get; set; }

    public int? TransactionType { get; set; }

    public string Airtime_Result_Type { get; set; }

    public bool? Airtime_Received { get; set; }

    public string Document_No { get; set; }

    public decimal? Airtime_Float_Balance { get; set; }

    public string From_MSISDN { get; set; }

    public string Remarks { get; set; }

    public string databaseName => DBName;

    public string tableName => DBTableName;

    public static string DBName = "SACCO";
    public static string DBTableName = "MsaccoAirtimeTopup";
  }
}
