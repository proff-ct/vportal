using System;
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
  [TestFixture]
    public class GetClientMSaccoUtilityPaymentTrxListForToday
    {
      private MSaccoUtilityPaymentBLL _utilityPaymentBLL = new MSaccoUtilityPaymentBLL();
      private dynamic _enUtilityPayment;
      private List<MSaccoUtilityPayment> _listMsaccoUtilityPayments = new List<MSaccoUtilityPayment>();

      [Test]
      public void returns_enumerable_of_client_utility_payment_transactions_for_the_current_day_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();

        _enUtilityPayment = _utilityPaymentBLL.GetMSaccoUtilityPaymentTrxListForClient(
          corporateNo, out int lastPage);

        Assert.IsInstanceOf<IEnumerable<MSaccoUtilityPayment>>(_enUtilityPayment);
        _listMsaccoUtilityPayments = (List<MSaccoUtilityPayment>)_enUtilityPayment;

        if (_listMsaccoUtilityPayments.Count > 0)
        {
          _listMsaccoUtilityPayments.ForEach(utilityPayment =>
          {
            Assert.AreEqual(
              DateTime.Now.Date, utilityPayment.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
          });
        }
      }
      [Test]
      public void returns_enumerable_of_client_utility_payment_transactions_for_the_current_day_when_called_with_pagination()
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
        _listMsaccoUtilityPayments = (List<MSaccoUtilityPayment>)_enUtilityPayment;

        if (_listMsaccoUtilityPayments.Count > 0)
        {
          _listMsaccoUtilityPayments.ForEach(utilityPayment =>
          {
            Assert.AreEqual(
              DateTime.Now.Date, utilityPayment.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
          });
        }
      }
    }
  }

}
