using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL
{
  public class Guarantors : IDBModel
  {
    public static string DBName = "SACCO";
    public static string DBTableName = "Guarantors";
    public string databaseName => DBName;
    public string tableName => DBTableName;
    public long Id { get; set; }

    public string Loan_Type { get; set; }

    public string Source { get; set; }

    public string Guarantor { get; set; }

    public string Session { get; set; }

    public double? Amount { get; set; }

    public string Status { get; set; }

    public DateTime? Datetime { get; set; }

    public string Corporate { get; set; }

    public string G_Account { get; set; }

    public string G_Account_Name { get; set; }

    public double? Amount_Guaranteed { get; set; }

    public string Remarks { get; set; }

    public string G_Phone_No { get; set; }

    public bool? Sent_Sms { get; set; }

    public string Guarantor_Session { get; set; }

    public bool? Posted { get; set; }

    public string Comments { get; set; }

    public DateTime? Date_Responded { get; set; }

    public DateTime? Date_sms_sent { get; set; }

    public bool? Updated_To_Client { get; set; }

    
  }

}
