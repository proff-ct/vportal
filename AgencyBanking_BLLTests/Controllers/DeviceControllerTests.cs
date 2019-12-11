using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgencyBanking_BLL.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AgencyBanking_BLL.Controllers.Tests
{
    [TestClass()]
    public class DeviceControllerTests
    {
        private DeviceController controller;

        [TestInitialize()]
        public void SetVariables()
        {
            controller = new DeviceController();
        }
        [TestMethod()]
        public void IndexTest()
        {
            ViewResult result = controller.Index() as ViewResult;
            if (result != null) Assert.AreEqual(result.ViewName,"create");
        }
    }
}