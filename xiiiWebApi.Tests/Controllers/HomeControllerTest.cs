using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xiiiWebApi;
using xiiiWebApi.Controllers;

namespace xiiiWebApi.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
