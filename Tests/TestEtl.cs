using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;
using Sqloogle.Operations;
using Sqloogle.Operations.Support;

namespace Tests {
    [TestFixture]
    public class TestEtl : EtlProcessHelper {

        private readonly KeyCheckOperation _keyCheck = new KeyCheckOperation(new[] { "SqlScript", "Database", "Schema", "Name", "Path", "SqlType", "CreateDate", "ModifyDate" });

        [Test]
        public void TestFakeOperation() {
            var rows = new List<Row> {
                new Row {{"k1", "v1"}, {"k2", "v2"}},
                new Row {{"k1", "v3"}, {"k2", "v4"}}
            };

            var fakeOperation = new FakeOperation(rows);

            var results = TestOperation(
                fakeOperation,
                new LogOperation()
            );

            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void TestUnionAllOperations() {
            var results = TestOperation(
                new ParallelUnionAllOperation().Add(
                    new FakeOperation(
                        new Row { { "key1", "value1" }, { "key2", "value2" } },
                        new Row { { "key1", "value3" }, { "key2", "value4" } }
                    ),
                    new FakeOperation(
                        new Row { { "key1", "value1" }, { "key2", "value2" } },
                        new Row { { "key1", "value3" }, { "key2", "value4" } },
                        new Row { { "key1", "value5" }, { "key2", "value6" } }
                    )
                )
                , new LogOperation()
            );

            Assert.AreEqual(5, results.Count);
        }

        [Test]
        public void TestUnionAllOperations2() {
            var results = TestOperation(
                new ParallelUnionAllOperation().Add(
                    new FakeOperation(
                        new Row { { "key1", "value1" }, { "key2", "value2" } },
                        new Row { { "key1", "value3" }, { "key2", "value4" } }
                    ),
                    new FakeOperation(
                        new Row { { "key1", "value1" }, { "key2", "value2" } },
                        new Row { { "key1", "value3" }, { "key2", "value4" } },
                        new Row { { "key1", "value5" }, { "key2", "value6" } }
                    )
                )
                , new LogOperation()
            );

            Assert.AreEqual(5, results.Count);
        }


        [Test]
        public void TestUnionAllPartials() {
            var unionAll = new ParallelUnionAllOperation()
                .Add(new PartialProcessOperation()
                    .Register(
                        new FakeOperation(
                            new Row { { "k1", "v1" }, { "k2", "v2" } },
                            new Row { { "k1", "v3" }, { "k2", "v4" } }
                    )))
                .Add(new PartialProcessOperation()
                    .Register(
                        new FakeOperation(
                            new Row { { "k1", "v1" }, { "k2", "v2" } },
                            new Row { { "k1", "v3" }, { "k2", "v4" } },
                            new Row { { "key1", "value5" }, { "key2", "value6" } }
                    )));

            var results = TestOperation(
                unionAll
                , new LogOperation()
            );

            Assert.AreEqual(5, results.Count);
        }

        [Test]
        public void TestUnionAllPartials2() {
            var unionAll = new ParallelUnionAllOperation()
                .Add(new PartialProcessOperation()
                    .Register(
                        new FakeOperation(
                            new Row { { "k1", "v1" }, { "k2", "v2" } },
                            new Row { { "k1", "v3" }, { "k2", "v4" } }
                    )))
                .Add(new PartialProcessOperation()
                    .Register(
                        new FakeOperation(
                            new Row { { "k1", "v1" }, { "k2", "v2" } },
                            new Row { { "k1", "v3" }, { "k2", "v4" } },
                            new Row { { "key1", "value5" }, { "key2", "value6" } }
                    )));

            var results = TestOperation(
                unionAll
                , new LogOperation()
            );

            Assert.AreEqual(5, results.Count);
        }


        [Test]
        public void TestUnionAllMixed() {
            var unionAll = new ParallelUnionAllOperation()
                .Add(new PartialProcessOperation()
                    .Register(
                        new FakeOperation(
                            new Row { { "k1", "v1" }, { "k2", "v2" } },
                            new Row { { "k1", "v3" }, { "k2", "v4" } }
                    )))
                .Add(new FakeOperation(
                        new Row { { "k1", "v1" }, { "k2", "v2" } },
                        new Row { { "k1", "v3" }, { "k2", "v4" } },
                        new Row { { "key1", "value5" }, { "key2", "value6" } }
                ));

            var results = TestOperation(
                unionAll
                , new LogOperation()
            );

            Assert.AreEqual(5, results.Count);

        }

        [Test]
        public void TestUnionAllMixed2() {
            var unionAll = new ParallelUnionAllOperation()
                .Add(new PartialProcessOperation()
                    .Register(
                        new FakeOperation(
                            new Row { { "k1", "v1" }, { "k2", "v2" } },
                            new Row { { "k1", "v3" }, { "k2", "v4" } }
                    )))
                .Add(new FakeOperation(
                        new Row { { "k1", "v1" }, { "k2", "v2" } },
                        new Row { { "k1", "v3" }, { "k2", "v4" } },
                        new Row { { "key1", "value5" }, { "key2", "value6" } }
                ));

            var results = TestOperation(
                unionAll
                , new LogOperation()
            );

            Assert.AreEqual(5, results.Count);

        }


        [Test]
        public void TestUnion() {

            var rows = new List<Row> {
                new Row {{"k1", "v1"}, {"k2", 1}},
                new Row {{"k1", "v1"}, {"k2", 1}},
                new Row {{"k1", "v1"}, {"k2", 2}}
            };

            var fakeOperation1 = new FakeOperation(rows);

            var groupByColumns = new string[] { "k1", "k2" };

            var results = TestOperation(
                fakeOperation1,
                new UnionOperation(groupByColumns),
                new LogOperation()
            );

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(2, results[0]["Count"]);
        }

        [Test]
        public void TestTransformCachedSql() {
            var rows = new List<Row> {
                new Row {{"SqlScript", "SET NOCOUNT ON;"}, {"Use", 1000}},
                new Row {{"SqlScript", "select * from toast where done = 1;"}, {"Use", 10}},
                new Row {{"SqlScript", "select * from [toast] where done = 2"}, {"Use", 5}},
                new Row {{"SqlScript", "select some, butter from dbo.toast where done = 'yes';"}, {"Use", 7}}
            };

            var results = TestOperation(
                new FakeOperation(rows),
                new CachedSqlPreTransform(),
                new LogOperation()
            );

            Assert.AreEqual(3, results.Count, "First record is noise, only 3 make it though.");
            Assert.AreEqual("select * from toast where done = @Parameter", results[0]["SqlScript"], "1; should be @Parameter.");
            Assert.AreEqual("select * from toast where done = @Parameter", results[1]["SqlScript"], "brackets should be removed, and 2 should be @Parameter.");
            Assert.AreEqual("select some, butter from toast where done = @Parameter", results[2]["SqlScript"], "dbo. should be removed, 'yes'; should be @Parameter");
        }

        [Test]
        public void TestAggregateCachedSql() {

            var rows = new List<Row> {
                new Row {{"sqlscript", "select * from toast where done = @Parameter"}, {"type","AdHoc"}, {"use", 10}, {"database", "db1"}},
                new Row {{"sqlscript", "select * from toast where done = @Parameter"}, {"type","AdHoc"}, {"use", 5}, {"database", "db2"}},
                new Row {{"sqlscript", "select some, butter from toast where done = @Parameter"}, {"type","AdHoc"}, {"use", 7}, {"database", "db1"}}
            };

            var results = TestOperation(
                new FakeOperation(rows),
                new CachedSqlAggregate(),
                new LogOperation()
            );

            Assert.AreEqual(2, results.Count, "Grouping on SqlScript, so only 2 records should make it through.");
            Assert.AreEqual(15, results[0]["Use"], "First 2 record's use should sum to 15.");
            Assert.AreEqual(7, results[1]["Use"], "Last record's use should stay at 7.");
        }

        [Test]
        public void TestReportingServicesTransform() {
            var rows = new List<Row> {
                new Row {{"Rdl", System.IO.File.ReadAllText(@"Files\Report.rdl")}, {"Name", "Report"}, {"Path", "Folder"}}
            };

            var results = TestOperation(
                new FakeOperation(rows),
                new ReportingServicesTransform(),
                new LogOperation().Ignore("Rdl")
            );

            Assert.AreEqual(3, results.Count);
        }

        [Test]
        public void TestCachedSqlProcess() {
            var cachedSqlProcess = new PartialProcessOperation()
                .Register(new FakeOperation(new List<Row> {
                    new Row {{"SqlScript", "SET NOCOUNT ON;"}, {"Use", 1000}, {"ObjectType", "Prepared"}},
                    new Row {{"SqlScript", "select * from toast where done = 1;"}, {"Use", 10}, {"ObjectType", "AdHoc"}},
                    new Row {{"SqlScript", "select * from [toast] where done = 2"}, {"Use", 5}, {"ObjectType", "AdHoc"}},
                    new Row {{"SqlScript", "select some, butter from dbo.toast where done = 'yes';"}, {"Use", 7}, {"ObjectType", "Prepared"}}
                }))
                .Register(new CachedSqlPreTransform())
                .Register(new CachedSqlAggregate())
                .Register(new CachedSqlPostTransform())
                .Register(_keyCheck);

            TestOperation(cachedSqlProcess);

        }

        [Test]
        public void TestSqloogleAggregate() {
            var maxDate = DateTime.MaxValue;
            var nowDate = DateTime.Now;

            var testData = new FakeOperation(
                new Row { { "sqlscript", "ss1" }, { "use", (long)1 }, { "created", DateTime.Now }, { "server", "s1" }, { "dropped", false }, {"database","db1"},{"schema","s1"}, {"name","n1"} },
                new Row { { "sqlscript", "ss1" }, { "use", (long)2 }, { "created", maxDate }, { "server", "s2" }, { "dropped", false }, { "database", "db1" }, { "schema", "s1" }, { "name", "n1" } },
                new Row { { "sqlscript", "ss1" }, { "use", (long)3 }, { "created", DateTime.Now }, { "server", "s3" }, { "dropped", false }, { "database", "db1" }, { "schema", "s1" }, { "name", "n1" } },
                new Row { { "sqlscript", "ss2" }, { "use", (long)4 }, { "created", nowDate }, { "server", "s4" }, { "dropped", false }, { "database", "db1" }, { "schema", "s1" }, { "name", "n1" } }
            );

            var results = TestOperation(testData, new SqloogleAggregate());

            var row1 = results[0];
            var row1Servers = ((HashSet<string>)row1["server"]);
            var row2 = results[1];
            var row2Servers = ((HashSet<string>)row2["server"]);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(6, row1["use"]);
            Assert.AreEqual(4, row2["use"]);
            Assert.AreEqual("ss1", row1["sqlscript"]);
            Assert.AreEqual("ss2", row2["sqlscript"]);
            Assert.AreEqual(maxDate, row1["created"]);
            Assert.AreEqual(nowDate, row2["created"]);
            Assert.AreEqual(3, row1Servers.Count);
            Assert.AreEqual(1, row2Servers.Count);
            Assert.IsTrue(row1Servers.Contains("s1"));
            Assert.IsTrue(row2Servers.Contains("s4"));

        }

    }
}

