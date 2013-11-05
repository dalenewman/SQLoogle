using NUnit.Framework;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Options;

namespace Tests.Integration {
    [TestFixture]
    public class TestDpDiff {
        [Test]
        public void TestGenerator() {

            var generate = new Generate {
                ConnectionString = "Server=localhost;Database=NorthWind;Trusted_Connection=True;",
                Options = new SqlOption {
                    Ignore = {
                        FilterAssemblies = !true,
                        FilterCLRFunction = !true,
                        FilterCLRStoreProcedure = !true,
                        FilterCLRTrigger = !true,
                        FilterCLRUDT = !true,
                        FilterDDLTriggers = !true,
                        FilterPartitionFunction = !true,
                        FilterPartitionScheme = !true,
                        FilterUsers = !true,
                        FilterXMLSchema = !true
                    }
                }
            };

            var results = generate.Process();
            Assert.AreEqual(14, results.Procedures.Count);
            Assert.AreEqual(0, results.Users.Count);
        }

    }
}
