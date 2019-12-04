using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NUnit.Framework;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ViewModels;
using VisibilityPortal_BLL.Utilities.MSSQLOperators;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL.Tests
{
  namespace CoretecClientBLL_Functions
  {
    [TestFixture]
    public class GetListOfClientsWithModules
    {
      private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
      private dynamic _output;

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
        config =>
        {
          config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
          config.CreateMap<CoreTecClient, CoretecClientWithModule>().ReverseMap();
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
      public void Returns_enumerable_of_CoretecClientWithModule_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();

        _output = _coretecClientBLL.GetListOfClientsWithModules(out int lastPage);

        Assert.IsInstanceOf<IEnumerable<CoretecClientWithModule>>(
          _output, "IEnumerable of CoretecClientWithModule NOT RETURNED!!!");
      }
      [Test]
      public void Returns_enumerable_of_CoretecClientWithModule_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 5;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _output = _coretecClientBLL.GetListOfClientsWithModules(out int lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<CoretecClientWithModule>>(
          _output, "IEnumerable of CoretecClientWithModule NOT RETURNED!!!");
        Assert.LessOrEqual(_output.Count, pageSize, "Records returned > Expected!");
        //Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }

    }
    
    [TestFixture]
    public class GetListOfClientModules
    {
      private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
      private dynamic _output;

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
        config =>
        {
          config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
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
      public void Returns_enumerable_of_CoretecClientModuleViewModel_when_called_without_pagination()
      {
        string corporateNo = 525201.ToString();

        _output = _coretecClientBLL.GetListOfClientModules(out int lastPage);

        Assert.IsInstanceOf<IEnumerable<CoretecClientModuleViewModel>>(
          _output, "IEnumerable of CoretecClientModuleViewModel NOT RETURNED!!!");
      }
      [Test]
      public void Returns_enumerable_of_CoretecClientModuleViewModel_when_called_with_pagination()
      {
        string corporateNo = 525201.ToString();
        int page = 1;
        int pageSize = 5;
        PaginationParameters pagingParams = new PaginationParameters(page, pageSize, null);

        _output = _coretecClientBLL.GetListOfClientModules(out int lastPage, true, pagingParams);

        Assert.IsInstanceOf<IEnumerable<CoretecClientModuleViewModel>>(
          _output, "IEnumerable of CoretecClientModuleViewModel NOT RETURNED!!!");
        Assert.LessOrEqual(_output.Count, pageSize, "Records returned > Expected!");
        //Assert.Greater(lastPage, 0, "last_page parameter value NOT greater than zero!");
      }

    }

    [TestFixture]
    public class GetUnregisteredClients
    {
      private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
      private dynamic _output;


      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
        config =>
        {
          config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
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
      public void Returns_enumerable_of_CoreTecClient_for_saccos_not_registered_on_portal()
      {
        /**
         * TO-DO:
         * + Write a better test for this to allow verification of the db result set
        **/
        _output = _coretecClientBLL.GetUnregisteredClients();

        Assert.IsInstanceOf<IEnumerable<CoreTecClient>>(
          _output, "IEnumerbale of CoreTecClient NOT RETURNED!!");

      }
    }
    [TestFixture]
    public class GetRegisteredClients
    {
      private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
      private dynamic _output;


      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
        config =>
        {
          config.CreateMap<Sacco, CoreTecClient>().ReverseMap();
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
      public void Returns_enumerable_of_CoreTecClient_for_saccos_that_ARE_REGISTERED_on_portal()
      {
        /**
         * TO-DO:
         * + Write a better test for this to allow verification of the db result set
        **/
        _output = _coretecClientBLL.GetRegisteredClients();

        Assert.IsInstanceOf<IEnumerable<CoreTecClient>>(
          _output, "IEnumerbale of CoreTecClient NOT RETURNED!!");

      }
    }

    [TestFixture]
    public class GetPortalModuleForClient
    {
      private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
      private dynamic _output;

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
       config =>
       {
         config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
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
      public void Returns_PortalModuleForClient_record_given_the_clientModuleId()
      {
        PortalModuleForClient samplePortalModuleForClient = new PortalModuleForClient();
        PortalModuleForClient newPortalModuleForClient = new PortalModuleForClient
        {
          ClientCorporateNo = 525201.ToString(),
          CreatedBy = "Testing123",
          IsEnabled = true,
          PortalModuleName = "Random Portal For Update"
        };
        // First, check if there's an existing record.
        ClearExistingRecord(newPortalModuleForClient);

        _coretecClientBLL.Save(newPortalModuleForClient, ModelOperation.AddNew);
        samplePortalModuleForClient = _coretecClientBLL
          .GetListOfClientModules(out int lastPage)
          .Where(m => m.ClientCorporateNo == 525201.ToString())
          .Single();



        _output = _coretecClientBLL.GetPortalModuleForClient(samplePortalModuleForClient.ClientModuleId);

        Assert.IsInstanceOf<PortalModuleForClient>(_output, "PortalModuleForClient NOT RETURNED");
        Assert.AreEqual(
          samplePortalModuleForClient.ClientModuleId, 
          _output.ClientModuleId, 
          "ClientModuleId NOT THE SAME!!");

        ClearExistingRecord(samplePortalModuleForClient);
      }
      private void ClearExistingRecord(PortalModuleForClient clientModule)
      {
        PortalModuleForClient existingRecord = _coretecClientBLL
          .GetListOfClientModules(out int lastPage)
          .Where(m => m.PortalModuleName == clientModule.PortalModuleName).SingleOrDefault();

        if (existingRecord != null)
        {
          _coretecClientBLL.Delete(existingRecord.ClientModuleId);
        }
      }
    }

    [TestFixture]
    public class Save
    {
      private CoretecClientBLL _coretecClientBLL = new CoretecClientBLL();
      private PortalModuleForClient expectedOutput = new PortalModuleForClient();


      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        Mapper.Initialize(
       config =>
       {
         config.CreateMap<PortalModuleForClient, CoretecClientModuleViewModel>().ReverseMap();
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
      public void Saves_new_PortalModuleForClient_record_to_db_with_id_and_createdOn()
      {
        PortalModuleForClient newPortalModuleForClient = new PortalModuleForClient
        {
          ClientCorporateNo = 525201.ToString(),
          CreatedBy = "Testing123",
          IsEnabled = true,
          PortalModuleName = "Random Portal Name"
        };
        // First, check if there's an existing record.
        ClearExistingRecord(newPortalModuleForClient);

        _coretecClientBLL.Save(newPortalModuleForClient, ModelOperation.AddNew);
        expectedOutput = _coretecClientBLL
          .GetListOfClientModules(out int lastPage)
          .Where(m => m.ClientCorporateNo == 525201.ToString())
          .Single();

        Assert.NotNull(expectedOutput.ClientModuleId, "ClientModuleId is NULL!!");
        Assert.NotNull(expectedOutput.CreatedOn, "CreatedOn IS NULL!!!!");

        // Delete the added record from the db
        ClearExistingRecord(expectedOutput);

      }
      [Test]
      public void Updates_existing_PortalModuleForClient_record_in_db()
      {
        // retrieve data from db
        // modify data
        // save to db
        // retrieve data from db
        // check that retrieved data == modified data
        PortalModuleForClient newPortalModuleForClient = new PortalModuleForClient
        {
          ClientCorporateNo = 525201.ToString(),
          CreatedBy = "Testing123",
          IsEnabled = true,
          PortalModuleName = "Random Portal Name"
        };

        // First, check if there's an existing record.
        ClearExistingRecord(newPortalModuleForClient);

        _coretecClientBLL.Save(newPortalModuleForClient, ModelOperation.AddNew);
        expectedOutput = _coretecClientBLL
          .GetListOfClientModules(out int lastPage)
          .Where(m => m.ClientCorporateNo == 525201.ToString())
          .Single();

        string updatedModifiedBy = "UpdatedModifyBy";
        expectedOutput.IsEnabled = false;
        expectedOutput.ModifiedBy = updatedModifiedBy;
        _coretecClientBLL.Save(expectedOutput, ModelOperation.Update);
        PortalModuleForClient updated = _coretecClientBLL
          .GetListOfClientModules(out int lastPage2)
          .Where(p => p.ClientModuleId == expectedOutput.ClientModuleId).Single();

        Assert.AreEqual(updatedModifiedBy, updated.ModifiedBy, "Modified By Not updated!!");

        // Delete the added record from the db
        ClearExistingRecord(updated);
      }
      private void ClearExistingRecord(PortalModuleForClient clientModule)
      {
        PortalModuleForClient existingRecord = _coretecClientBLL
          .GetListOfClientModules(out int lastPage)
          .Where(m => m.PortalModuleName == clientModule.PortalModuleName).SingleOrDefault();

        if (existingRecord != null)
        {
          _coretecClientBLL.Delete(existingRecord.ClientModuleId);
        }
      }
    }
  }

}
