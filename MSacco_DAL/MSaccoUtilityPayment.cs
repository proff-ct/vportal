using System;

namespace MSacco_DAL
{
  public class MSaccoUtilityPayment : IDBModel
  {
    public int Entry_No { get; set; }

    public string SESSION_ID { get; set; }

    public string Status { get; set; }

    public string Account_No { get; set; }

    public string Account_Name { get; set; }

    public string Utility_Payment_Type { get; set; }

    public string Utility_Payment_Account { get; set; }

    public string Telephone_No { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Date_Created { get; set; }

    public DateTime? Time_Created { get; set; }

    public DateTime? Transaction_Date { get; set; }

    public decimal? Account_Balance { get; set; }

    public bool? Request_Confirmed { get; set; }

    public bool? Sent_To_Journal { get; set; }

    public DateTime? Date_Sent_To_Journal { get; set; }

    public DateTime? Time_Sent_To_Journal { get; set; }

    public string Corporate_No { get; set; }

    public string Comments { get; set; }

    public string Response_Status { get; set; }

    public string Response_Value { get; set; }

    public DateTime? Datetime { get; set; }

    public bool? Source { get; set; }

    public string Utility_Result_Type { get; set; }

    public bool? Utility_Received { get; set; }

    public string Document_No { get; set; }

    public decimal? Utility_Float_Balance { get; set; }

    public string Remarks { get; set; }

    public string Token { get; set; }

    public string Stream { get; set; }

    //public string databaseName => DBName;

    //public string tableName => DBTableName;

    public static string DBName = "SACCO";
    public static string DBTableName = "[Msacco Utility Payment]";
  }
}
