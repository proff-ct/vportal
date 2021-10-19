using CallCenter_Dataspecs.Models;
using CallCenter_Dataspecs.USSDRequests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL
{
  public class USSDRequest : IDBModel, IUSSDRequest
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "UssdRequests";
    //public string databaseName => DBName;
    //public string tableName => DBTableName;

    public int Entry_No { get; set; }
    public string User_Input { get; set; }
    public string Reply { get; set; }
    public string Session_ID { get; set; }
    public DateTime? DateTime { get; set; }
    public string Telephone_No { get; set; }
    public string Corporate_No { get; set; }
  }
}
