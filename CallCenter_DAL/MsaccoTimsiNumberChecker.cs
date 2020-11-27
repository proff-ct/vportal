using CallCenter_Dataspecs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL
{
  public class MsaccoTimsiNumberChecker : IDBModel, IMSACCO_TIMSI_NumberChecker
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "MsaccoTimsiNumberChecker";
    public string databaseName => DBName;
    public string tableName => DBTableName;


    public int Id { get; set; }
    public string TimsiId { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime DateLinked { get; set; }
    public string Comments { get; set; }
  }
}
