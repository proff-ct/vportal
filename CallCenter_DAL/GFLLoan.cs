using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL
{
  public class GFLLoan : MSaccoSalaryAdvance, IDBModel
  {
    public new static string DBTableName = "Loan";
    public new string tableName => DBTableName;
  }
}
