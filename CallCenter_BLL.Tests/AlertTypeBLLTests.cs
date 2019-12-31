using System.Collections.Generic;
using System.Linq;
using CallCenter_DAL;
using NUnit.Framework;

namespace CallCenter_BLL.Tests
{
  namespace AlertTypeBLL_Functions
  {
    [TestFixture]
    public class GetAllAlertTypes
    {
      private readonly List<AlertType> _alertTypeList = new List<AlertType>();
      private AlertTypeBLL _alertTypeBLL = new AlertTypeBLL();

      [Test]
      public void Returns_IEnumerable_of_AlertType_containing_alertTypeName()
      {
        IEnumerable<AlertType> output = _alertTypeBLL.GetAllAlertTypes();

        Assert.IsInstanceOf<IEnumerable<AlertType>>(output, "AlertType IEnumerable not returned!");
        Assert.GreaterOrEqual(output.Count(), 1, "AlertType list count less than 1");

        output.ToList().ForEach(i =>
        {
          Assert.IsNotNull(i.AlertName, "NULL AlertType Name encountered!");
          Assert.IsNotNull(i.Id, "NULL AlertType Id");
        });
      }
    }
    [TestFixture]
    public class GetAlertTypeByName
    {
      private AlertTypeBLL _alertTypeBLL = new AlertTypeBLL();
      [Test]
      public void Returns_a_AlertType_record_of_specified_name()
      {
        string alertTypeName = AlertType.AlertTypes.WARNING_ALERT.ToString();

        var output = _alertTypeBLL.GetAlertTypeByName(alertTypeName);

        Assert.IsInstanceOf<AlertType>(output, "AlertType object not returned!");
        Assert.IsNotNull(output.AlertName, "AlertType Name is null!");
      }
    }
  }

}
