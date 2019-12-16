using System.Collections.Generic;
using AutoMapper;
using MSacco_BLL.Models;
using MSacco_BLL.MSSQLOperators;
using MSacco_DAL;
using NUnit.Framework;

namespace MSacco_BLL.Tests
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
      private int lastPage = 0;

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
    [TestFixture]
    public class GetLinkInfoWithLinkDowntimeForClient
    {
      private LinkMonitoringBLL linkMonitoringBLL = new LinkMonitoringBLL();
      private dynamic _enLinkStatusPlusDowntimes;

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
        config =>
        {
          config.CreateMap<LinkMonitoring, LinkStatusPlusDowntime>();
        });
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        Mapper.Reset();
      }

      #endregion

      [Test]
      public void Returns_link_status_record_with_IEnumerable_of_downtime_records_for_specified_client()
      {
        string corporateNo = 525201.ToString();
        _enLinkStatusPlusDowntimes = linkMonitoringBLL.GetLinkInfoWithLinkDowntimeForClient(
          corporateNo);
        Assert.IsInstanceOf<LinkStatusPlusDowntime>(
          _enLinkStatusPlusDowntimes, "NOT LinkStatusPlusDowntimes!!!");
        Assert.IsInstanceOf<IEnumerable<LinkDowntime>>(
          _enLinkStatusPlusDowntimes.Downtimes, "IEnumerable of Downtimes NOT returned!!");
      }
    }

    [TestFixture]
    public class GetLinkInfoWithLinkDowntimeForAllClients
    {
      private LinkMonitoringBLL _linkMonitoringBLL = new LinkMonitoringBLL();
      private dynamic _enLinkStatusPlusDowntimes;

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
        config =>
        {
          config.CreateMap<LinkMonitoring, LinkStatusPlusDowntime>();
        });
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        Mapper.Reset();
      }

      #endregion
      private int lastPage = 0;

      [Test]
      public void Returns_enumerable_of_link_info_containing_downtime_records_when_called_without_pagination()
      {
        _enLinkStatusPlusDowntimes = _linkMonitoringBLL.GetLinkInfoWithLinkDowntimeForAllClients(out lastPage);

        Assert.IsInstanceOf<IEnumerable<LinkStatusPlusDowntime>>(_enLinkStatusPlusDowntimes);
      }
      [Test]
      public void Returns_enumerable_of_link_ino_containing_downtime_records_when_called_with_pagination()
      {
        int page = 1;
        int pageSize = 10;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _enLinkStatusPlusDowntimes = _linkMonitoringBLL.GetLinkInfoWithLinkDowntimeForAllClients(
          out lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<LinkStatusPlusDowntime>>(_enLinkStatusPlusDowntimes);
        Assert.LessOrEqual(_enLinkStatusPlusDowntimes.Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }

    }
  }

}
