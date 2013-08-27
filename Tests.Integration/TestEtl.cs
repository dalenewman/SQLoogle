using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations;
using Sqloogle.Operations.Support;

namespace Tests.Integration {
    [TestFixture]
    public class TestEtl : EtlProcessHelper {

        [Test]
        public void TestExtractDatabases() {
            var results = TestOperation(
                new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;"),
                new LogOperation()
            );

            Assert.Less(0, results.Count);
        }


        [Test]
        public void TestExtractAndFilterDatabases() {

            var d1 = new PartialProcessOperation();
            d1.Register(new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;"));
            d1.Register(new DatabaseFilter());
            d1.Register(new LogOperation());

            var d2 = new PartialProcessOperation();
            d2.Register(new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;"));
            d2.Register(new DatabaseFilter());
            d2.Register(new LogOperation());

            var union = new SerialUnionAllOperation(d1,d2);

            var results = TestOperation(
                union
            );

            Assert.Less(0, results.Count);
        }

        [Test]
        public void TestExtractDatabasesUnionAll() {
            var results = TestOperation(
                new ParallelUnionAllOperation()
                    .Add(new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;"))
                    .Add(new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;")),
                new DatabaseFilter(),
                new LogOperation()
            );

            Assert.Less(0, results.Count);
        }

        [Test]
        public void TestExtractDatabasesUnionAllThenUnion() {

            var groupByColumns = new string[] { "DatabaseId", "Database", "CompatibilityLevel" };

            var results = TestOperation(
                new ParallelUnionAllOperation()
                    .Add(new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;"))
                    .Add(new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;")),
                new UnionOperation(groupByColumns),
                new DatabaseFilter(),
                new LogOperation()
            );

            Assert.Less(0, results.Count);
        }

        [Test]
        public void TestReportingServicesExtract() {
            var results = TestOperation(
                new DatabaseExtract("Server=localhost;Database=master;Trusted_Connection=True;"),
                new ReportingServicesExtract(),
                new ReportingServicesTransform(),
                new LogOperation()
            );

            Assert.LessOrEqual(0, results.Count);
        }

        [Test]
        public void TestTableStatsExtract() {
            var results = TestOperation(
                new TableStatsExtract("Server=localhost;Database=master;Trusted_Connection=True;"),
                new LogOperation()
            );
            Assert.LessOrEqual(0, results.Count);
        }

        [Test]
        public void TestIndexStatsExtract() {
            var results = TestOperation(
                new IndexStatsExtract("Server=localhost;Database=master;Trusted_Connection=True;"),
                new LogOperation()
            );
            Assert.LessOrEqual(0, results.Count);
        }

        [Test]
        public void TestCachedObjectsExtract() {
            var results = TestOperation(
                new CachedObjectStatsExtract("Server=localhost;Database=master;Trusted_Connection=True;")
            );
            Assert.LessOrEqual(0, results.Count);
        }

        [Test]
        public void TestExtractDefinitionFromOneDatabase() {
            const string connectionString = "Server=localhost;Database=Awesome;Trusted_Connection=True;";
            var databases = new List<Row>() {
                new Row() {{"ConnectionString", connectionString}, {"Database", "Awesome"}, {"DatabaseId", 5}, {"CompatibilityLevel", 90}}
            };

            var results = TestOperation(
                new FakeOperation(databases),
                new DefinitionExtract()
            );
            Assert.Less(0, results.Count);
        }

        [Test]
        public void TestExtractCachedSql() {
            var results = TestOperation(
                new CachedSqlExtract("Server=localhost;Database=master;Trusted_Connection=True;"),
                new CachedSqlPreTransform(),
                new LogOperation()
            );
            Assert.LessOrEqual(0, results.Count);
        }

        [Test]
        public void TestSqlAgentJobExtract() {
            var results = TestOperation(
                new SqlAgentJobExtract("Server=localhost;Database=master;Trusted_Connection=True;"),
                new LogOperation()
            );

            Assert.Less(0, results.Count);
        }

    }
}

