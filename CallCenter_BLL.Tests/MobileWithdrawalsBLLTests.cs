using System.Collections.Generic;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace MobileWithdrawalsBLL_Functions
  {
    [TestFixture]
    public class GetMobileWithdrawalsTrxListForClient
    {
      private MobileWithdrawalsBLL _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
      private dynamic _enMobileWithdrawals;
      [Test]
      public void Returns_enumerable_of_client_mobile_withdrawal_transactions_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();

        _enMobileWithdrawals = _mobileWithdrawalsBLL.GetMobileWithdrawalsTrxListForClient(
          corporateNo, out int lastPage);

        Assert.IsInstanceOf<IEnumerable<MobileWithdrawals>>(_enMobileWithdrawals);
      }
      [Test]
      public void Returns_enumerable_of_client_mobile_withdrawals_transactions_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 20;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enMobileWithdrawals = _mobileWithdrawalsBLL.GetMobileWithdrawalsTrxListForClient(
          corporateNo, out int lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<MobileWithdrawals>>(_enMobileWithdrawals);
        Assert.LessOrEqual(_enMobileWithdrawals.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }
    }
    [TestFixture]
    public class GetLatestWithdrawalForClient
    {
      private MobileWithdrawalsBLL _mobileWithdrawalsBLL = new MobileWithdrawalsBLL();
      private readonly string corporateNo = 525201.ToString();

      [Test]
      public void Returns_latest_mobile_withdrawal_record_for_specified_client()
      {

        MobileWithdrawals output = _mobileWithdrawalsBLL.GetLatestWithdrawalForClient(corporateNo);

        Assert.IsInstanceOf<MobileWithdrawals>(output, "Mobile Withdrawal instance not returned!");
        Assert.AreEqual(output.Corporate_No, corporateNo, "Corporate No MISMATCH!!");
      }
    }
  }

}
