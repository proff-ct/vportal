using System.Collections.Generic;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace LinkMonitoringBLL_Functions
  {
    [TestFixture]
    public class GetLinkInfoForClient
    {
      private LinkMonitoringBLL _linkMonitoringBLL = new LinkMonitoringBLL();
      private dynamic _expectedLinkMonitoring;
      [Test]
      public void Returns_link_monitoring_record_for_specified_client()
      {
        string corporateNo = 525201.ToString();

        _expectedLinkMonitoring = _linkMonitoringBLL.GetLinkInfoForClient(corporateNo);

        Assert.IsInstanceOf<LinkMonitoring>(_expectedLinkMonitoring);
        Assert.AreEqual(corporateNo, _expectedLinkMonitoring.Corporate_No, "CorporateNo NOT MATCHING!!");
      }
    }
  [TestFixture]
    public class GetLinkInfoForAllClients
    {
      private LinkMonitoringBLL _linkMonitoringBLL = new LinkMonitoringBLL();
      private dynamic _expectedLinkMonitoring;
      int lastPage = 0;

      [Test]
      public void Returns_enumerable_of_link_monitoring_records_when_called_without_pagination()
      {
        _expectedLinkMonitoring = _linkMonitoringBLL.GetLinkInfoForAllClients(out lastPage);

        Assert.IsInstanceOf<IEnumerable<LinkMonitoring>>(_expectedLinkMonitoring);
      }
      [Test]
      public void Returns_enumerable_of_link_monitoring_records_when_called_with_pagination()
      {
        int page = 1;
        int pageSize = 10;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _expectedLinkMonitoring = _linkMonitoringBLL.GetLinkInfoForAllClients(
          out lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<LinkMonitoring>>(_expectedLinkMonitoring);
        Assert.LessOrEqual(_expectedLinkMonitoring.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }

    }
  }

}
