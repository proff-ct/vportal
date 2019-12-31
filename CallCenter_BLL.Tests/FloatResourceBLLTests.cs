using System.Collections.Generic;
using System.Linq;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace FloatResourceBLL_Functions
  {
    [TestFixture]
    public class GetFloatResourcesList
    {
      private readonly List<FloatResource> floatResourceList = new List<FloatResource>();
      private FloatResourceBLL _floatResourceBLL = new FloatResourceBLL();

      [Test]
      public void Returns_IEnumerable_of_FloatResource_containing_floatResourceName()
      {
        IEnumerable<FloatResource> output = _floatResourceBLL.GetFloatResourcesList();

        Assert.IsInstanceOf<IEnumerable<FloatResource>>(output, "FloatResource IEnumerable not returned!");
        Assert.GreaterOrEqual(output.Count(), 1, "FloatResource list count less than 1");

        output.ToList().ForEach(r =>
        {
          Assert.IsNotNull(r.ResourceName, "NULL FloatResource Name encountered!");
          Assert.IsTrue(!string.IsNullOrEmpty(r.Id), "NULL Resource Id");
        });
      }
    }
    [TestFixture]
    public class GetFloatResourceByName
    {
      private FloatResourceBLL _floatResourceBLL = new FloatResourceBLL();
      [Test]
      public void Returns_a_FloatResource_record_of_specified_name()
      {
        string floatResourceName = FloatResource.FloatResources.B2C_MPESA.ToString();

        var output = _floatResourceBLL.GetFloatResourceByName(floatResourceName);

        Assert.IsInstanceOf<FloatResource>(output, "FloatResource object not returned!");
        Assert.IsNotNull(output.ResourceName, "FloatResource Name is null!");
      }
    }
  }

}
