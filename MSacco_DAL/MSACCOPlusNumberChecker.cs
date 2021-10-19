using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Models;

namespace MSacco_DAL
{
  public class MsaccoPlusNumberChecker : IMsaccoPlusNumberChecker, IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "MsaccoPlusNumberChecker";
    //public string databaseName => DBName;
    //public string tableName => DBTableName;

    public int Id { get; set; }

    public string DeviceId { get; set; }

    public string PhoneNumber { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime DateLinked { get; set; }

    public string Comments { get; set; }
  }

}
