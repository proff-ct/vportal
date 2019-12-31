using System.Collections.Generic;
using System.Linq;
using CallCenter_BLL.MSSQLOperators;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace FloatResourceAlertForClientBLL_Functions
  {
    [TestFixture]
    public class GetListOfFloatResourceAlertsForClient
    {
      private readonly List<FloatResourceAlertForClient> _clientFloatResourceAlertList = new List<FloatResourceAlertForClient>();
      private FloatResourceAlertForClientBLL _floatResourceAlertForClientBLL = new FloatResourceAlertForClientBLL();

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        _clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo",
          FloatResourceId = "TestingFR2D",
          AlertTypeId = 1,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        });
        _clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo",
          FloatResourceId = "TestingFR1D",
          AlertTypeId = 2,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        });
        _clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo2",
          FloatResourceId = "TestingFR1D",
          AlertTypeId = 3,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        });
        _clientFloatResourceAlertList.ForEach(r => ClearExistingRecord(r));
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        _clientFloatResourceAlertList.ForEach(r => ClearExistingRecord(r));
      }
      private void ClearExistingRecord(FloatResourceAlertForClient clientFloatResourceAlert)
      {
        FloatResourceAlertForClient existingRecord = _floatResourceAlertForClientBLL
          .GetListOfFloatResourceAlertsForClient(clientFloatResourceAlert.ClientCorporateNo)
          .Where(m =>
            m.ClientCorporateNo == clientFloatResourceAlert.ClientCorporateNo
            && m.FloatResourceId == clientFloatResourceAlert.FloatResourceId
            && m.AlertTypeId == clientFloatResourceAlert.AlertTypeId
            ).SingleOrDefault();

        if (existingRecord != null)
        {
          _floatResourceAlertForClientBLL.Delete(existingRecord.Id.ToString());
        }
      }

      #endregion

      [Test]
      public void Returns_IEnumerable_of_FloatResourceAlertForClient_containing_ClientCorporateNo_FloatResourceId_Threshold_and_TriggerCondition()
      {
        IEnumerable<FloatResourceAlertForClient> output = _floatResourceAlertForClientBLL.GetListOfFloatResourceAlertsForClient(_clientFloatResourceAlertList.First().ClientCorporateNo);

        Assert.IsInstanceOf<IEnumerable<FloatResourceAlertForClient>>(
          output, "FloatResourceAlertForClient IEnumerable not returned!");
      }
    }
    [TestFixture]
    public class GetListOfFloatResourceAlertsForAllClients
    {
      private readonly List<FloatResourceAlertForClient> _clientFloatResourceAlertList = new List<FloatResourceAlertForClient>();
      private FloatResourceAlertForClientBLL _floatResourceAlertForClientBLL = new FloatResourceAlertForClientBLL();

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        _clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo",
          FloatResourceId = "TestingFR2D",
          AlertTypeId = 1,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        });
        _clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo1",
          FloatResourceId = "TestingFR1D",
          AlertTypeId = 2,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        });
        _clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo3",
          FloatResourceId = "TestingFR1D",
          AlertTypeId = 3,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        });
        _clientFloatResourceAlertList.ForEach(r => ClearExistingRecord(r));
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        _clientFloatResourceAlertList.ForEach(r => ClearExistingRecord(r));
      }
      private void ClearExistingRecord(FloatResourceAlertForClient clientFloatResourceAlert)
      {
        FloatResourceAlertForClient existingRecord = _floatResourceAlertForClientBLL
          .GetListOfFloatResourceAlertsForAllClients(out int lastPage)
          .Where(m =>
            m.ClientCorporateNo == clientFloatResourceAlert.ClientCorporateNo
            && m.FloatResourceId == clientFloatResourceAlert.FloatResourceId
            && m.AlertTypeId == clientFloatResourceAlert.AlertTypeId
            ).SingleOrDefault();

        if (existingRecord != null)
        {
          _floatResourceAlertForClientBLL.Delete(existingRecord.Id.ToString());
        }
      }

      #endregion

      [Test]
      public void Returns_IEnumerable_of_FloatResourceAlertForClient_of_same_number_as_created_when_called_without_pagination()
      {
        // create the items
        _clientFloatResourceAlertList.ForEach(
          r =>
          {
            ClearExistingRecord(r);
            _floatResourceAlertForClientBLL.Save(r, ModelOperation.AddNew);
          });

        IEnumerable<FloatResourceAlertForClient> output = _floatResourceAlertForClientBLL.GetListOfFloatResourceAlertsForAllClients(
          out int lastPage);

        Assert.AreEqual(
          _clientFloatResourceAlertList.Count, output.ToList().Count, "List Count Mismatch!!");
      }
      [Test]
      public void Returns_IEnumerable_of_FloatResourceAlertForClient_when_called_with_pagination()
      {
        int page = 1;
        int pageSize = 2;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);
        // create the items
        _clientFloatResourceAlertList.ForEach(
          r =>
          {
            ClearExistingRecord(r);
            _floatResourceAlertForClientBLL.Save(r, ModelOperation.AddNew);
          });

        IEnumerable<FloatResourceAlertForClient> output = _floatResourceAlertForClientBLL.GetListOfFloatResourceAlertsForAllClients(
          out int lastPage, true, pagingParams);

        Assert.LessOrEqual(output.ToList().Count, pageSize, "Records returned > Expected!");
        Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }

    }

    [TestFixture]
    public class GetFloatResourceAlertForClientById
    {
      private readonly List<FloatResourceAlertForClient> _clientFloatResourceAlertList = new List<FloatResourceAlertForClient>();
      private FloatResourceAlertForClientBLL _clientFRABLL = new FloatResourceAlertForClientBLL();
      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        _clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo",
          FloatResourceId = "TestingGetByID",
          AlertTypeId = 1,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        });
        //_clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        //{
        //  ClientCorporateNo = "TestingCNo1",
        //  FloatResourceId = "TestingFR1D",
        //  AlertTypeId = 2,
        //  Threshold = 20.ToString(),
        //  TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
        //  CreatedBy = "Testing123"
        //});
        //_clientFloatResourceAlertList.Add(new FloatResourceAlertForClient
        //{
        //  ClientCorporateNo = "TestingCNo3",
        //  FloatResourceId = "TestingFR1D",
        //  AlertTypeId = 3,
        //  Threshold = 20.ToString(),
        //  TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
        //  CreatedBy = "Testing123"
        //});
        _clientFloatResourceAlertList.ForEach(r => ClearExistingRecord(r));
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        _clientFloatResourceAlertList.ForEach(r => ClearExistingRecord(r));
      }
      private void ClearExistingRecord(FloatResourceAlertForClient clientFloatResourceAlert)
      {
        FloatResourceAlertForClient existingRecord = _clientFRABLL
          .GetListOfFloatResourceAlertsForAllClients(out int lastPage)
          .Where(m =>
            m.ClientCorporateNo == clientFloatResourceAlert.ClientCorporateNo
            && m.FloatResourceId == clientFloatResourceAlert.FloatResourceId
            && m.AlertTypeId == clientFloatResourceAlert.AlertTypeId
            ).SingleOrDefault();

        if (existingRecord != null)
        {
          _clientFRABLL.Delete(existingRecord.Id.ToString());
        }
      }

      #endregion



      [Test]
      public void Returns_a_FloatResourceAlertForClient_record_of_specified_Id()
      {

        dynamic output = _clientFRABLL.GetFloatResourceAlertForClientById(1);

        Assert.IsInstanceOf<FloatResourceAlertForClient>(
          output, "FloatResourceAlertForClient object not returned!");
      }
    }
    [TestFixture]
    public class Save
    {

      private FloatResourceAlertForClientBLL _floatResourceAlertForClientBLL = new FloatResourceAlertForClientBLL();
      private FloatResourceAlertForClient expectedOutput = new FloatResourceAlertForClient();


      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
      }

      #endregion

      [Test]
      public void Saves_new_FloatResourceAlertForClient_record_to_db_with_createdOn()
      {
        FloatResourceAlertForClient newFloatResourceAlertForClient = new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo2",
          FloatResourceId = "TestingFR1D",
          AlertTypeId = 3,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123"
        };
        // First, check if there's an existing record.
        ClearExistingRecord(newFloatResourceAlertForClient);

        _floatResourceAlertForClientBLL.Save(newFloatResourceAlertForClient, ModelOperation.AddNew);
        expectedOutput = _floatResourceAlertForClientBLL
          .GetListOfFloatResourceAlertsForClient(newFloatResourceAlertForClient.ClientCorporateNo)
          .Where(m => m.ClientCorporateNo == newFloatResourceAlertForClient.ClientCorporateNo)
          .Single();

        Assert.AreEqual(newFloatResourceAlertForClient.ClientCorporateNo, expectedOutput.ClientCorporateNo, "ClientCorporateNo MISMATCH!!");
        Assert.AreEqual(newFloatResourceAlertForClient.FloatResourceId, expectedOutput.FloatResourceId, "FloatResourceId MISMATCH!!");
        Assert.AreEqual(
          newFloatResourceAlertForClient.AlertTypeId,
          expectedOutput.AlertTypeId, 
          "AlertTypeId MISMATCH!!");

        Assert.NotNull(expectedOutput.CreatedOn, "CreatedOn IS NULL!!!!");

        // Delete the added record from the db
        ClearExistingRecord(expectedOutput);

      }
      [Test]
      public void Updates_existing_FloatResourceAlertForClient_record_in_db()
      {
        // retrieve data from db
        // modify data
        // save to db
        // retrieve data from db
        // check that retrieved data == modified data
        FloatResourceAlertForClient newFloatResourceAlertForClient = new FloatResourceAlertForClient
        {
          ClientCorporateNo = "TestingCNo2",
          FloatResourceId = "TestingFR1DUpdated",
          AlertTypeId = 1,
          Threshold = 20.ToString(),
          TriggerCondition = AlertType.TriggerConditions.LESS_THAN.ToString(),
          CreatedBy = "Testing123ForUpdate"
        };

        // First, check if there's an existing record.
        ClearExistingRecord(newFloatResourceAlertForClient);

        _floatResourceAlertForClientBLL.Save(newFloatResourceAlertForClient, ModelOperation.AddNew);
        expectedOutput = _floatResourceAlertForClientBLL
          .GetListOfFloatResourceAlertsForClient(newFloatResourceAlertForClient.ClientCorporateNo)
          .Where(m => m.CreatedBy == newFloatResourceAlertForClient.CreatedBy)
          .Single();

        string updatedModifiedBy = "UpdatedModifyBy";
        expectedOutput.ModifiedBy = updatedModifiedBy;
        _floatResourceAlertForClientBLL.Save(expectedOutput, ModelOperation.Update);
        FloatResourceAlertForClient updated = _floatResourceAlertForClientBLL
          .GetListOfFloatResourceAlertsForClient(newFloatResourceAlertForClient.ClientCorporateNo)
          .Where(p => p.CreatedBy == expectedOutput.CreatedBy).Single();

        Assert.AreEqual(updatedModifiedBy, updated.ModifiedBy, "Modified By Not updated!!");

        // Delete the added record from the db
        ClearExistingRecord(updated);
      }
      private void ClearExistingRecord(FloatResourceAlertForClient clientFloatResourceAlert)
      {
        FloatResourceAlertForClient existingRecord = _floatResourceAlertForClientBLL
          .GetListOfFloatResourceAlertsForClient(clientFloatResourceAlert.ClientCorporateNo)
          .Where(m =>
            m.FloatResourceId == clientFloatResourceAlert.FloatResourceId
            && m.AlertTypeId == clientFloatResourceAlert.AlertTypeId
            && m.CreatedBy == clientFloatResourceAlert.CreatedBy
            ).SingleOrDefault();

        if (existingRecord != null)
        {
          _floatResourceAlertForClientBLL.Delete(existingRecord.Id.ToString());
        }
      }

    }
  }

}
