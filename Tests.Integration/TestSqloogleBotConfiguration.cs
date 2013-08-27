using NUnit.Framework;
using System.Configuration;
using Sqloogle;

namespace Tests.Integration
{
    [TestFixture]
    public class TestSqloogleBotConfiguration
    {
        [Test]
        public void TestGetAttribute()
        {
            var config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");

            Assert.AreEqual("c:\\Sqloogle\\SearchIndex\\", config.SearchIndexPath);
        }

        [Test]
        public void TestGetCollection()
        {
            var config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");
            Assert.AreEqual(1, config.Servers.Count);
            Assert.AreEqual("localhost", config.Servers[0].Name);
        }

    }
}
