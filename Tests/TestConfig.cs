using System.Configuration;
using NUnit.Framework;
using Sqloogle;

namespace Tests {

    [TestFixture]
    public class TestConfig {

        [Test]
        public void TestConfiguration() {

            var config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");
            Assert.AreEqual(@"c:\Sqloogle\SearchIndex\", config.SearchIndexPath);
        }
    }
}