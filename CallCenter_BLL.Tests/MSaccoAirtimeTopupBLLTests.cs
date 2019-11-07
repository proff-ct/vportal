using System.Collections;
using System.Collections.Generic;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace MSaccoUtilityPaymentBLL_Functions
  {
    [TestFixture]
    public class GetMSaccoAirtimeTopupTrxListForClient
    {
      private MSaccoAirtimeTopupBLL _airtimeTopupBLL = new MSaccoAirtimeTopupBLL();
      private dynamic _enAirtimeTopup;
      [Test]
      public void returns_enumerable_of_client_airtime_topup_transactions_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();

        _enAirtimeTopup = _airtimeTopupBLL.GetMSaccoAirtimeTopupTrxListForClient(
          corporateNo, out int lastPage);

        Assert.IsInstanceOf<IEnumerable<MSaccoAirtimeTopup>>(_enAirtimeTopup);
      }
      [Test]
      public void returns_enumerable_of_client_airtime_topup_transactions_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 20;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enAirtimeTopup = _airtimeTopupBLL.GetMSaccoAirtimeTopupTrxListForClient(
          corporateNo, out int lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<MSaccoAirtimeTopup>>(_enAirtimeTopup);
        Assert.LessOrEqual(_enAirtimeTopup.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }
    }
  }

}
