﻿namespace VisibilityPortal_DAL
{
  public class Sacco : IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "[Source Information]";

    public string corporateNo { get; set; }
    public string corporateNo_2 { get; set; }
    public string saccoName_1 { get; set; }

    //public string databaseName => DBName;
    //public string tableName => DBTableName;
  }
}
