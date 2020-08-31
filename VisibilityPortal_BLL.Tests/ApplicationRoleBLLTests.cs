using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
using VisibilityPortal_BLL.Models;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL.Tests
{
  namespace ApplicationRoleBLL_Functions
  {
    [TestFixture]
    public class GetApplicationRolesListAsync
    {
      private ApplicationRoleBLL _applicationRoleBLL = new ApplicationRoleBLL();
      public string passPhrase = "MyPassPhrase";

      public ApplicationRoleManager RoleManager;

      [OneTimeSetUp]
      public void InitTest()
      {
        Mock<IRoleStore<ApplicationRole>> mockRoleStore = new Mock<IRoleStore<ApplicationRole>>();
        mockRoleStore
          .Setup(r => r.FindByNameAsync(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
          .ReturnsAsync(new ApplicationRole()
          {
            Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
            SetupPassPhrase = passPhrase
          }
          //new ApplicationRole()
          //{
          //  Name = PortalUserRoles.SystemRoles.SystemAdmin.ToString(),
          //  SetupPassPhrase = passPhrase
          //},
          //new ApplicationRole()
          //{
          //  Name = PortalUserRoles.SystemRoles.Admin.ToString(),
          //  SetupPassPhrase = passPhrase
          //},
          //new ApplicationRole()
          //{
          //  Name = PortalUserRoles.SystemRoles.Regular.ToString(),
          //  SetupPassPhrase = passPhrase
          //}
          );

        RoleManager = new ApplicationRoleManager(mockRoleStore.Object);
      }
      [OneTimeTearDown]
      public void TestCleanup()
      {
        RoleManager.Dispose();
      }
      [Test]
      public void Returns_an_IEnumerable_of_the_application_roles()
      {
        var result = Task.Run(() =>
          _applicationRoleBLL.GetApplicationRolesListAsync(RoleManager)).Result;

        Assert.IsInstanceOf<IEnumerable<ApplicationRole>>(result, "IEnumerable not returned!!!");
      }
    }
  }
}
