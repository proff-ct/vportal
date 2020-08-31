using System.Collections.Generic;
using System.Linq;
using VisibilityPortal_DAL;
using NUnit.Framework;
using VisibilityPortal_Dataspecs.Models;

namespace VisibilityPortal_BLL.Tests
{
  namespace PortalModuleBLL_Functions
  {
    [TestFixture]
    public class GetModulesList
    {
      private readonly List<PortalModule> moduleList = new List<PortalModule>();
      private PortalModuleBLL portalModuleBLL = new PortalModuleBLL();

      [Test]
      public void Returns_IEnumerable_of_portal_modules_containing_moduleName_and_routeprefix()
      {
        IEnumerable<IPortalModule> output = portalModuleBLL.GetModulesList();

        Assert.IsInstanceOf<IEnumerable<PortalModule>>(output, "PortalModule IEnumerable not returned!");
        Assert.GreaterOrEqual(output.Count(), 1, "PortalModule list count less than 1");

        output.Select(s => new { s.ModuleName, s.RoutePrefix }).ToList().ForEach(s =>
        {
          Assert.IsNotNull(s.ModuleName, "NULL Module Name encountered!");
          Assert.IsTrue(!string.IsNullOrEmpty(s.RoutePrefix), "NULL Route Prefix");
        });
      }
    }
    public class GetModuleByName
    {
      private PortalModuleBLL portalModuleBLL = new PortalModuleBLL();
      [Test]
      public void Returns_a_portal_module_record_of_specified_name()
      {
        string moduleName = PortalModule.modules.CallCenter.ToString();

        var output = portalModuleBLL.GetModuleByName(moduleName);

        Assert.IsInstanceOf<PortalModule>(output, "PortalModule object not returned!");
        Assert.IsNotNull(output.ModuleName, "Module Name is null!");
      }
    }
  }

}
