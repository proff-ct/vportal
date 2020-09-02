using MSacco_Dataspecs.Models;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Functions
{
  public interface IBL_MobileWithdrawals
  {
    IMobileWithdrawals_SACCODB GetLatestWithdrawalForClient(string corporateNo);
    IEnumerable<IMobileWithdrawals_SACCODB> GetMobileWithdrawalsTrxListForClient(
      string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

    IEnumerable<IMobileWithdrawals_SACCODB> GetClientMobileWithdrawalsFinancialSummaryForToday(
      string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);
  }
}
