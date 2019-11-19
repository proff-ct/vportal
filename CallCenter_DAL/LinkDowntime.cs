using System;

namespace CallCenter_DAL
{
  public class LinkDowntime : IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "LinkDowntime";
    public string databaseName => DBName;
    public string tableName => DBTableName;
    public int Entry_No { get; set; }
    public DateTime? Downtime_Start { get; set; }
    public DateTime? Downtime_Stop { get; set; }
    public string Corporate_No { get; set; }
    public string Comment { get; set; }
    public string Url { get; set; }
    public DateTime? Last_Check { get; set; }
    public string Corporate_Name { get; set; }
    public string Downtime_Duration { get; set; }
  }
}