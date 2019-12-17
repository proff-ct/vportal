using System.Collections;
using System.Collections.Generic;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using NUnit.Framework;

namespace MSacco_BLL.Tests
{
  namespace MSaccoUtilityPaymentBLL_Functions
  {
    [TestFixture]
    public class GetMSaccoUtilityPaymentTrxListForClient
    {
      private MSaccoUtilityPaymentBLL _utilityPaymentBLL = new MSaccoUtilityPaymentBLL();
      private dynamic _enUtilityPayment;
      [Test]
      public void returns_enumerable_of_client_utility_payment_transactions_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();

        _enUtilityPayment = _utilityPaymentBLL.GetMSaccoUtilityPaymentTrxListForClient(
          corporateNo, out int lastPage);

        Assert.IsInstanceOf<IEnumerable<MSaccoUtilityPayment>>(_enUtilityPayment);
      }
      [Test]
      public void returns_enumerable_of_client_utility_payment_transactions_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 20;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enUtilityPayment = _utilityPaymentBLL.GetMSaccoUtilityPaymentTrxListForClient(
          corporateNo, out int lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<MSaccoUtilityPayment>>(_enUtilityPayment);
        Assert.LessOrEqual(_enUtilityPayment.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }
    }
  }

}
