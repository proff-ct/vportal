using System;

namespace MSacco_DAL
{
  public class LinkMonitoring : IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "LinkMonitoring";
    public int Entry_No { get; set; }

    public string Corporate_No { get; set; }

    public string Corporate_Name { get; set; }

    public string Ping_Result { get; set; }

    public string Http_Status { get; set; }

    public string Url { get; set; }

    public DateTime? Last_Check { get; set; }

    public DateTime? Last_Email_Sent { get; set; }

    //public string databaseName => DBName;

    //public string tableName => DBTableName;
  }
}
