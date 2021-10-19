using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_DAL
{
  public class PesalinkFloatBalance : IPesalinkFloatBalance, IDBModel
  {
    public static string DBName => "Pesalink";
    public static string DBTableName => "FLoatBalances";
    //public string databaseName => DBName;
    //public string tableName => DBTableName;
   
    public long ID { get; set; }
    public string CorporateNo { get; set; }
    public decimal Amount { get; set; }
    public DateTime Last_Updated { get; set; }
  }
}
