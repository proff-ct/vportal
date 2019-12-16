using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSacco_DAL;

namespace MSacco_BLL
{
  public class GuarantorsBLL
  {
    string _query;
    string _tblGuarantors = Guarantors.DBTableName;
    public IEnumerable<Guarantors> GetGuarantorsForLoan(string loanSessionID)
    {
      _query = $@"SELECT * FROM {_tblGuarantors} WHERE Session='{loanSessionID}'";
      return new DapperORM().QueryGetList<Guarantors>(_query);
    }
    public IEnumerable<Guarantors> GetGuarantorsForManyLoans(IEnumerable<string> loanSessionIDs)
    {
      _query = $@"SELECT * FROM {_tblGuarantors} WHERE Session IN ({string.Join(",", loanSessionIDs.Select(s => string.Format("'{0}'", s)))})";
      return new DapperORM().QueryGetList<Guarantors>(_query);
    }
  }

}
