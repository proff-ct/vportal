using System;
using System.Collections;
using System.Collections.Generic;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using NUnit.Framework;

namespace MSacco_BLL.Tests
{
  namespace MSaccoAirtimeTopupBLL_Functions
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
    [TestFixture]
    public class GetClientMSaccoAirtimeTopupTrxListForToday
    {
      private MSaccoAirtimeTopupBLL _airtimeTopupBLL = new MSaccoAirtimeTopupBLL();
      private dynamic _enAirtimeTopup;
      private List<MSaccoAirtimeTopup> _listMSaccoAirtimeTopups = new List<MSaccoAirtimeTopup>();

      [Test]
      public void returns_enumerable_of_client_airtime_topup_transactions_for_the_current_day_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();

        _enAirtimeTopup = _airtimeTopupBLL.GetClientMSaccoAirtimeTopupTrxListForToday(
          corporateNo, out int lastPage);

        Assert.IsInstanceOf<IEnumerable<MSaccoAirtimeTopup>>(_enAirtimeTopup);
        _listMSaccoAirtimeTopups = (List<MSaccoAirtimeTopup>)_enAirtimeTopup;

        if (_listMSaccoAirtimeTopups.Count > 0)
        {
          _listMSaccoAirtimeTopups.ForEach(airtimeTopup =>
          {
            Assert.AreEqual(
              DateTime.Now.Date, airtimeTopup.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
          });
        }
      }
      [Test]
      public void returns_enumerable_of_client_airtime_topup_transactions_for_the_current_day_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 20;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enAirtimeTopup = _airtimeTopupBLL.GetClientMSaccoAirtimeTopupTrxListForToday(
          corporateNo, out int lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<MSaccoAirtimeTopup>>(_enAirtimeTopup);
        Assert.LessOrEqual(_enAirtimeTopup.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
        _listMSaccoAirtimeTopups = (List<MSaccoAirtimeTopup>)_enAirtimeTopup;

        if (_listMSaccoAirtimeTopups.Count > 0)
        {
          _listMSaccoAirtimeTopups.ForEach(airtimeTopup =>
          {
            Assert.AreEqual(
              DateTime.Now.Date, airtimeTopup.Transaction_Date.Value.Date, "Returned DATE not EQUAL TO TODAY'S DATE!!!");
          });
        }
      }
    }
  }

}
