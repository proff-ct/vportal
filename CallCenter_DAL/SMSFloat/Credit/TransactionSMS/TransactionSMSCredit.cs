using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL.SMSFloat.Credit
{
  public class TransactionSMSCredit : IDBModel
  {

    public string databaseName => DBName;

    public string tableName => DBTableName;

    public static string DBName = "SACCO";
    public static string DBTableName = "[MSacco TopUp]";
  }
}
