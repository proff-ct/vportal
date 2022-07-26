﻿using System;

namespace CallCenter_DAL
{
  public class BulkSMSBalance : IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "BulkSMSBalances";
    //public string databaseName => DBName;
    //public string tableName => DBTableName;

    public int Entry_No { get; set; }

    public string Corporate_No { get; set; }

    public int? Balance { get; set; }

    public DateTime? Last_Updated { get; set; }

  }
}
