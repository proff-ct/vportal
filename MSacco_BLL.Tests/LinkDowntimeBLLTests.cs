using System.Collections.Generic;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using NUnit.Framework;

namespace MSacco_BLL.Tests
{
  namespace LinkDowntimeBLL_Functions
  {
    [TestFixture]
    public class GetDowntimeRecordsForClient
    {
      private LinkDowntimeBLL _linkDowntimeBLL = new LinkDowntimeBLL();
      private dynamic _expectedLinkDowntime;
      [Test]
      public void Returns_IEnumerable_of_link_downtime_records_for_specified_client()
      {
        string corporateNo = 525201.ToString();

        _expectedLinkDowntime = _linkDowntimeBLL.GetDowntimeRecordsForClient(corporateNo);

        Assert.IsInstanceOf<IEnumerable<LinkDowntime>>(_expectedLinkDowntime);
        foreach (LinkDowntime record in _expectedLinkDowntime)
        {
          Assert.AreEqual(corporateNo, record.Corporate_No, "CorporateNo NOT MATCHING!!");
        }
      }
    }
    [TestFixture]
    public class GetDowntimeRecordsForAllClients
    {
      private LinkDowntimeBLL _linkDowntimeBLL = new LinkDowntimeBLL();
      private dynamic _expectedLinkDowntime;
      private int lastPage = 0;

      [Test]
      public void Returns_enumerable_of_link_downtime_records_when_called_without_pagination()
      {
        _expectedLinkDowntime = _linkDowntimeBLL.GetDowntimeRecordsForAllClients(out lastPage);

        Assert.IsInstanceOf<IEnumerable<LinkDowntime>>(_expectedLinkDowntime);
      }
      [Test]
      public void Returns_enumerable_of_link_downtime_records_when_called_with_pagination()
      {
        int page = 1;
        int pageSize = 10;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _expectedLinkDowntime = _linkDowntimeBLL.GetDowntimeRecordsForAllClients(
          out lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<LinkDowntime>>(_expectedLinkDowntime);
        Assert.LessOrEqual(_expectedLinkDowntime.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }

    }
  }

}
