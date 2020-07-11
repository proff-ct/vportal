using System.Collections.Generic;
using CallCenter_Dataspecs.MSSQLOperators;
using CallCenter_Dataspecs.Models;

namespace CallCenter_Dataspecs.Functions
{
  public interface IBL_MobileWithdrawals
  {
    IMobileWithdrawals_SACCODB GetLatestWithdrawalForClient(string corporateNo);
    IEnumerable<IMobileWithdrawals_SACCODB> GetMobileWithdrawalsTrxListForClient(
      string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);
  }
}