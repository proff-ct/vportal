using System.Collections.Generic;
using System.Linq;
using VisibilityPortal_DAL;
using NUnit.Framework;

namespace VisibilityPortal_BLL.Tests
{
  namespace PortalUserRoleBLL_Functions
  {
    [TestFixture]
    public class GetListOfPortalUserRoles
    {
      private PortalUserRoleBLL _portalUserRoleBLL = new PortalUserRoleBLL();
      private List<PortalUserRole> _samplePortalUserRoles = new List<PortalUserRole>();

      dynamic expectedOutput;

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        _samplePortalUserRoles.Add(new PortalUserRole {
          UserId = "TestingUser123",
          AspRoleId = "TestingRole123",
          ClientModuleId = "Testing123"
        });
        _samplePortalUserRoles.Add(new PortalUserRole {
          UserId = "TestingUser124",
          AspRoleId = "TestingRole124",
          ClientModuleId = "Testing124"
        });
        _samplePortalUserRoles.Add(new PortalUserRole {
          UserId = "TestingUser125",
          AspRoleId = "TestingRole125",
          ClientModuleId = "Testing125"
        });
        _samplePortalUserRoles.ForEach(r => ClearExistingRecord(r));
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        _samplePortalUserRoles.ForEach(r => ClearExistingRecord(r));
      }
      private void ClearExistingRecord(PortalUserRole portalUserRole)
      {
        PortalUserRole existingRecord = _portalUserRoleBLL
          .GetListOfPortalUserRoles()
          .Where(m =>
            m.AspRoleId == portalUserRole.AspRoleId
            && m.ClientModuleId == portalUserRole.ClientModuleId
            && m.UserId == portalUserRole.UserId
            ).SingleOrDefault();

        if (existingRecord != null)
        {
          _portalUserRoleBLL.Delete(existingRecord.ClientModuleId);
        }
      }

      #endregion

      [Test]
      public void Returns_IEnumerable_of_PortalUserRoles()
      {
        expectedOutput = _portalUserRoleBLL.GetListOfPortalUserRoles();

        Assert.IsInstanceOf<IEnumerable<PortalUserRole>>(
          expectedOutput, "IEnumerable of PortalUserRole NOT RETURNED!!!");
      }
    }

    [TestFixture]
    public class GetPortalUserRoleListForUser
    {
      private readonly List<PortalUserRole> _portalUserRoleList = new List<PortalUserRole>();
      private PortalUserRoleBLL _portalUserRoleBLL = new PortalUserRoleBLL();

      #region TestPreparation

      [OneTimeSetUp]
      public void ClassInit()
      {
        // Executes once for the test class. (Optional) 
        _portalUserRoleList.Add(new PortalUserRole
        {
          UserId = "TestingUser123",
          AspRoleId = "TestingRole123",
          ClientModuleId = "Testing123"
        });
        _portalUserRoleList.Add(new PortalUserRole
        {
          UserId = "TestingUser123",
          AspRoleId = "TestingRole124",
          ClientModuleId = "Testing123"
        });
        _portalUserRoleList.Add(new PortalUserRole
        {
          UserId = "TestingUser121",
          AspRoleId = "TestingRole123",
          ClientModuleId = "Testing123"
        });
        _portalUserRoleList.ForEach(r => ClearExistingRecord(r));
      }
      [OneTimeTearDown]
      public void ClassCleanup()
      {
        // Runs once after all tests in this class are executed. (Optional)
        // Not guaranteed that it executes instantly after all tests from the class.
        _portalUserRoleList.ForEach(r => ClearExistingRecord(r));
      }
      private void ClearExistingRecord(PortalUserRole portalUserRole)
      {
        PortalUserRole existingRecord = _portalUserRoleBLL
          .GetListOfPortalUserRoles()
          .Where(m =>
            m.AspRoleId == portalUserRole.AspRoleId
            && m.ClientModuleId == portalUserRole.ClientModuleId
            && m.UserId == portalUserRole.UserId
            ).SingleOrDefault();

        if (existingRecord != null)
        {
          _portalUserRoleBLL.Delete(existingRecord.ClientModuleId);
        }
      }

      #endregion

      [Test]
      public void Returns_IEnumerable_of_portalUserRoles_containing_userId_roleId_and_portalUserRoleId()
      {
        IEnumerable<PortalUserRole> output = _portalUserRoleBLL.GetPortalUserRoleListForUser(_portalUserRoleList.First().UserId);

        Assert.IsInstanceOf<IEnumerable<PortalUserRole>>(
          output, "PortalUserRole IEnumerable not returned!");
      }
}
    [TestFixture]
    public class Save
    {

      private PortalUserRoleBLL _portalUserRoleBLL = new PortalUserRoleBLL();
      private PortalUserRole expectedOutput = new PortalUserRole();


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
      public void Saves_new_PortalUserRole_record_to_db_with_createdOn()
      {
        PortalUserRole newPortalUserRole = new PortalUserRole
        {
          UserId = "TestingUser123",
          AspRoleId = "TestingRole123",
          ClientModuleId = "Testing123",
          CreatedBy = "TestRunNUNIT"
        };
        // First, check if there's an existing record.
        ClearExistingRecord(newPortalUserRole);

        _portalUserRoleBLL.Save(newPortalUserRole, ModelOperation.AddNew);
        expectedOutput = _portalUserRoleBLL
          .GetListOfPortalUserRoles()
          .Where(m => m.UserId == newPortalUserRole.UserId)
          .Single();

        Assert.AreEqual(newPortalUserRole.UserId, expectedOutput.UserId, "UserId MISMATCH!!");
        Assert.AreEqual(newPortalUserRole.AspRoleId, expectedOutput.AspRoleId, "RoleId MISMATCH!!");
        Assert.AreEqual(
          newPortalUserRole.ClientModuleId, expectedOutput.ClientModuleId, "ClientModuleId MISMATCH!!");

        Assert.NotNull(expectedOutput.CreatedOn, "CreatedOn IS NULL!!!!");

        // Delete the added record from the db
        ClearExistingRecord(expectedOutput);

      }
      [Test]
      public void Updates_existing_PortalUserRole_record_in_db()
      {
        // retrieve data from db
        // modify data
        // save to db
        // retrieve data from db
        // check that retrieved data == modified data
        PortalUserRole newPortalUserRole = new PortalUserRole
        {
          UserId = "TestingUser123",
          AspRoleId = "TestingRole123",
          ClientModuleId = "Testing123"
        };

        // First, check if there's an existing record.
        ClearExistingRecord(newPortalUserRole);

        _portalUserRoleBLL.Save(newPortalUserRole, ModelOperation.AddNew);
        expectedOutput = _portalUserRoleBLL
          .GetListOfPortalUserRoles()
          .Where(m => m.UserId == newPortalUserRole.UserId)
          .Single();

        string updatedModifiedBy = "UpdatedModifyBy";
        expectedOutput.ModifiedBy = updatedModifiedBy;
        _portalUserRoleBLL.Save(expectedOutput, ModelOperation.Update);
        PortalUserRole updated = _portalUserRoleBLL
          .GetListOfPortalUserRoles()
          .Where(p => p.UserId == expectedOutput.UserId).Single();

        Assert.AreEqual(updatedModifiedBy, updated.ModifiedBy, "Modified By Not updated!!");

        // Delete the added record from the db
        ClearExistingRecord(updated);
      }
      private void ClearExistingRecord(PortalUserRole portalUserRole)
      {
        PortalUserRole existingRecord = _portalUserRoleBLL
          .GetListOfPortalUserRoles()
          .Where(m => 
            m.AspRoleId == portalUserRole.AspRoleId
            && m.ClientModuleId == portalUserRole.ClientModuleId
            && m.UserId == portalUserRole.UserId
            ).SingleOrDefault();

        if (existingRecord != null)
        {
          _portalUserRoleBLL.Delete(existingRecord.Id.ToString());
        }
      }

    }
  }

}
