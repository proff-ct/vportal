using System;

namespace CallCenter_DAL.SMSFloat.Credit
{
  //from dbo.SACCO.[Sacco Bulksms Topup]
  // credit record for Sacco BULK SMS
  public class BulkSMSCredit : IDBModel
  {
    public int Id { get; set; }

    public string Sacco { get; set; }

    public long? SmsCount { get; set; }

    public DateTime? Datetime { get; set; }

    public string Comments { get; set; }

    public string UserID { get; set; }

    public string databaseName => DBName;

    public string tableName => DBTableName;

    public static string DBName = "SACCO";
    public static string DBTableName = "[Sacco Bulksms Topup]";
  }
}
