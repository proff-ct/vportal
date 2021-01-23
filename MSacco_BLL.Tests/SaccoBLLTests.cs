using System.Collections.Generic;
using System.Linq;
using MSacco_DAL;
using NUnit.Framework;

namespace MSacco_BLL.Tests
{
  namespace SaccoBLL_Functions
  {
    [TestFixture]
    public class GetSaccoList
    {
      private readonly List<Sacco> saccoList = new List<Sacco>();
      private SaccoBLL saccoBLL = new SaccoBLL();

      [OneTimeSetUp]
      public void InitTest()
      {

        //Mock<IRoleStore<ApplicationRole>> mockRoleStore = new Mock<IRoleStore<ApplicationRole>>();
        //mockRoleStore
        //  .Setup(r => r.FindByNameAsync(PortalUserRoles.SystemRoles.SuperAdmin.ToString()))
        //  .ReturnsAsync(new ApplicationRole()
        //  {
        //    Name = PortalUserRoles.SystemRoles.SuperAdmin.ToString(),
        //    SetupPassPhrase = passPhrase
        //  });

        //RoleManager = new ApplicationRoleManager(mockRoleStore.Object);
      }
      [OneTimeTearDown]
      public void TestCleanup()
      {
      }

      [Test]
      public void Returns_IEnumerable_of_saccos_containing_saccoName_and_corporate_no()
      {
        //IEnumerable<Sacco> output = saccoBLL.GetSaccoList();
        var output = saccoBLL.GetSaccoList();

        Assert.IsInstanceOf<IEnumerable<Sacco>>(output, "Sacco IEnumerable not returned!");
        Assert.GreaterOrEqual(output.Count(), 1, "Sacco list count less than 1");

        output.Select(s => new { s.corporateNo, s.saccoName_1 }).ToList().ForEach(s =>
        {
          Assert.IsNotNull(s.corporateNo, "NULL Corporate Number encountered!");
          Assert.IsTrue(!string.IsNullOrEmpty(s.saccoName_1), "NULL Sacco Name");
        }
        );
      }
    }
  }

}
