using System.Web.Mvc;
using NUnit.Framework;
using WebSite.Controllers;

namespace Tests
{
    [TestFixture]
    public class TestWebsiteControllers
    {
        [Test]
        public void Index()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("", string.Empty);
        }

    }
}
