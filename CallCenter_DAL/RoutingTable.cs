using CallCenter_Dataspecs.MSACCOCustomer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL
{
  public class RoutingTable : IDBModel, IRouting_Table
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "Routing Table";
    //public string databaseName => DBName;
    //public string tableName => DBTableName;

    public string Account_No { get; set; }
    public string Comments { get; set; }
    public string Corporate_No { get; set; }
    public DateTime? Date_Blocked { get; set; }
    public DateTime? DateRegistered { get; set; }
    public int Entry_No { get; set; }
    public bool? GFEDHATest { get; set; }
    public string IMEI { get; set; }
    public bool? Is_Agent { get; set; }
    public string Language_Code { get; set; }
    public string Language_Code_2 { get; set; }
    public DateTime? Last_Login_Date { get; set; }
    public DateTime? Last_Pin_Change_Date { get; set; }
    public int? No_Pin_Attempt { get; set; }
    public string PIN_No_Changed { get; set; }
    public bool? PinChanged { get; set; }
    public string SMS_Code { get; set; }
    public bool? SMS_Code_Verified { get; set; }
    public int? Status { get; set; }
    public bool Subscribed { get; set; }
    public string Telephone_No { get; set; }
    public double? Withdrawal_Limit_daily { get; set; }
    public double? Withdrawal_Limit_Transaction { get; set; }
  }
}
