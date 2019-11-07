using System.Collections.Generic;
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
      [Test]
      public void Returns_IEnumerable_of_link_monitoring_records_for_all_clients()
      {
        _expectedLinkMonitoring = _linkMonitoringBLL.GetLinkInfoForAllClients();

        Assert.IsInstanceOf<IEnumerable<LinkMonitoring>>(
          _expectedLinkMonitoring, "LinkMonitoring IEnumerable not returned!");
        Assert.GreaterOrEqual(
          _expectedLinkMonitoring.Count, 2, "LinkMonitoring list count less than 2");
      }
    }
  }

}
