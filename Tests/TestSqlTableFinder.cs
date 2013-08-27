using System.Linq;
using NUnit.Framework;
using Sqloogle.Operations;

namespace Tests
{
    [TestFixture]
    public class TestSqlTableFinder {

        [Test]
        public void TestTable() {
            const string sql = "select * from table;";
            var tables = CachedSqlPreTransform.FindTables(sql);

            Assert.AreEqual("table", tables.ElementAt(0));
        }

        [Test]
        public void TestTables()
        {
            const string sql = "select * from table1 t1 inner join table2 t2 on (t1.id = t2.id);";

            var tables = CachedSqlPreTransform.FindTables(sql);

            Assert.AreEqual("table1", tables.ElementAt(0));
            Assert.AreEqual("table2", tables.ElementAt(1));
        }

        [Test]
        public void TestTableInSubQuery()
        {
            const string sql = @"
select * 
from table1 t1
inner join table2 t2 on (t1.id = t2.id)
inner join (
    select id
    from table3
    where crazy = 'yes'
) x on (t2.id = x.id);";

            var tables = CachedSqlPreTransform.FindTables(sql);

            Assert.AreEqual("table1", tables.ElementAt(0));
            Assert.AreEqual("table2", tables.ElementAt(1));
            Assert.AreEqual("table3", tables.ElementAt(2));
        }

        [Test]
        public void TestTableWithBrackets() {
            const string sql = "select * from [table];";
            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual("table", tables.ElementAt(0));
        }

        [Test]
        public void TestTableWithSchema() {
            const string sql = "select * from dbo.table;";
            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual("dbo.table", tables.ElementAt(0));
        }

        [Test]
        public void TestTableWithDatabaseAndSchema()
        {
            const string sql = "select * from database.dbo.table;";
            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual("database.dbo.table", tables.ElementAt(0));
        }

        [Test]
        public void TestTableWithServerDatabaseAndSchema() 
        {
            const string sql = "select * from [server].[database].[dbo].[table];";
            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual("server.database.dbo.table", tables.ElementAt(0));
        }

        [Test]
        public void TestDistinct() {
            const string sql = "select * from table1 t1 inner join table1 t2 on (t1.id = t2.id);";
            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual("table1", tables.ElementAt(0));
        }

        [Test]
        public void TestTableWithOldTimeDoubleQuotes()
        {
            const string sql = "select * from \"dbo\".\"table\"";
            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual("dbo.table", tables.ElementAt(0));
        }

        [Test]
        public void TestTableNameWithSpaces()
        {
            const string sql = "select * from [my table]";
            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual("[my table]", tables.ElementAt(0));
        }

        [Test]
        public void TestSchemaDefault()
        {
            const string sql = "select * from table";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("dbo", schemas.ElementAt(0));
        }

        [Test]
        public void TestSchemaDbo()
        {
            const string sql = "select * from dbo.table";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("dbo", schemas.ElementAt(0));
        }

        [Test]
        public void TestSchemaSchema()
        {
            const string sql = "select * from schema.table";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("schema", schemas.ElementAt(0));
        }

        [Test]
        public void TestSchemaDatabaseSchema()
        {
            const string sql = "select * from database.schema.table";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("schema", schemas.ElementAt(0));
        }

        [Test]
        public void TestSchemaServerDatabaseSchema()
        {
            const string sql = "select * from server.database.schema.table;";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("schema", schemas.ElementAt(0));
        }

        [Test]
        public void TestSchemaWithBracketsAndAlias()
        {
            const string sql = "select * from [schema].table t1;";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("schema", schemas.ElementAt(0));
        }

        [Test]
        public void TestSchemaWithDoubleQuotes()
        {
            const string sql = "select * from \"schema\".\"table\"";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("schema", schemas.ElementAt(0));
        }

        [Test]
        public void TestTwoTablesSameDefaultSchema()
        {
            const string sql = "select * from table1 t1 inner join table2 t2 on (t1.id = t2.id);";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("dbo", schemas.ElementAt(0));
        }

        [Test]
        public void TestTwoTablesSameDboAndDefaultSchema()
        {
            const string sql = "select * from dbo.table1 t1 inner join table2 t2 on (t1.id = t2.id);";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(1, schemas.Count());
            Assert.AreEqual("dbo", schemas.ElementAt(0));
        }

        [Test]
        public void TestTwoTablesMixedSchema()
        {
            const string sql = "select * from schema.table1 t1 inner join table2 t2 on (t1.id = t2.id);";
            var schemas = CachedSqlPreTransform.FindSchemas(sql);
            Assert.AreEqual(2, schemas.Count());
            Assert.AreEqual("schema", schemas.ElementAt(0));
            Assert.AreEqual("dbo", schemas.ElementAt(1));
        }

        [Test]
        public void TestNoDatabases()
        {
            const string sql = "select * from schema.table1";
            var databases = CachedSqlPreTransform.FindDatabases(sql);
            Assert.AreEqual(0, databases.Count());
        }

        [Test]
        public void TestOneDatabase()
        {
            const string sql = "select * from database.schema.table";
            var databases = CachedSqlPreTransform.FindDatabases(sql);
            Assert.AreEqual(1, databases.Count());
            Assert.AreEqual("database", databases.ElementAt(0));
        }

        [Test]
        public void TestTwoDatabases()
        {
            const string sql = "select * from database1.schema.table t1 inner join database2.schema.table t2 on (t1.id = t2.id)";
            var databases = CachedSqlPreTransform.FindDatabases(sql);
            Assert.AreEqual(2, databases.Count());
            Assert.AreEqual("database1", databases.ElementAt(0));
            Assert.AreEqual("database2", databases.ElementAt(1));
        }

        [Test]
        public void TestTwoTablesWithoutDatabases()
        {
            const string sql = "select * from table t1 inner join schema.table t2 on (t1.id = t2.id)";
            var databases = CachedSqlPreTransform.FindDatabases(sql);
            Assert.AreEqual(0, databases.Count());
        }

        [Test]
        public void TestCommonSchemaNoDatabase()
        {
            const string sql = "select * from table";
            var schema = CachedSqlPreTransform.FindCommonSchema(sql);
            var database = CachedSqlPreTransform.FindCommonDatabase(sql);
            Assert.AreEqual("dbo", schema);
            Assert.AreEqual(string.Empty, database);
        }

        [Test]
        public void TestCommonSchemaAndDatabase()
        {
            const string sql = "select * from database.schema.table";
            var schema = CachedSqlPreTransform.FindCommonSchema(sql);
            var database = CachedSqlPreTransform.FindCommonDatabase(sql);
            Assert.AreEqual("schema", schema);
            Assert.AreEqual("database", database);
        }

        [Test]
        public void TestCommonSchemaAndNotCommonDatabase()
        {
            const string sql = "select * from database1.schema.table t1 inner join database2.schema.table t2 on (t1.id = t2.id)";
            var schema = CachedSqlPreTransform.FindCommonSchema(sql);
            var database = CachedSqlPreTransform.FindCommonDatabase(sql);
            Assert.AreEqual("schema", schema);
            Assert.AreEqual(string.Empty, database);
        }

        [Test]
        public void TestNotCommonSchemaAndNotCommonDatabase()
        {
            const string sql = "select * from database1.schema1.table t1 inner join database2.schema2.table t2 on (t1.id = t2.id)";
            var schema = CachedSqlPreTransform.FindCommonSchema(sql);
            var database = CachedSqlPreTransform.FindCommonDatabase(sql);
            Assert.AreEqual(string.Empty, schema);
            Assert.AreEqual(string.Empty, database);
        }

        [Test]
        public void TestCommonSchemaAndCommonDatabaseDefined()
        {
            const string sql = "select * from database.schema.table t1 inner join database.schema.table t2 on (t1.id = t2.id)";

            var schema = CachedSqlPreTransform.FindCommonSchema(sql);
            var database = CachedSqlPreTransform.FindCommonDatabase(sql);
            Assert.AreEqual("schema", schema);
            Assert.AreEqual("database", database);
        }

        [Test]
        public void TestOneExcludedDatabase()
        {
            const string sql = "select * from AdventureWorks.dbo.table t1 inner join some.table t2 on (t1.id = t2.id)";
            var schema = CachedSqlPreTransform.FindCommonSchema(sql);
            var database = CachedSqlPreTransform.FindCommonDatabase(sql);
            Assert.AreEqual("", schema);
            Assert.AreEqual("AdventureWorks", database);
        }

        [Test]
        public void TestNoTables()
        {
            const string sql = "set nocount on;";

            var schema = CachedSqlPreTransform.FindCommonSchema(sql);
            var database = CachedSqlPreTransform.FindCommonDatabase(sql);
            Assert.AreEqual(string.Empty, schema);
            Assert.AreEqual(string.Empty, database);
        }

        [Test]
        public void TestSimplifyTableNames() {
            const string sql = @"select * from ""dbo"".""table"" ""why"" inner join [dbo].[table] [t] on why.id = t.id;";
            var cleanSql = CachedSqlPreTransform.RemoveExtraDoubleQuotesAndBrackets(sql);
            Assert.AreEqual(@"select * from dbo.table why inner join dbo.table [t] on why.id = t.id;", cleanSql);
        }

        [Test]
        public void TestProblem() {
            const string sql = @"
SELECT *
FROM    ""ScopeApp1"".""dbo"".""vr_03681d"" ""vr_03681d"" LEFT OUTER JOIN ""ScopeApp1"".""dbo"".""Currncy"" ""Currncy"" ON ""vr_03681d"".""CuryID""=""Currncy"".""CuryId""
WHERE RI_ID = @Parameter
";

            var tables = CachedSqlPreTransform.FindTables(sql);
            Assert.AreEqual(2, tables.Count());
            Assert.AreEqual(@"ScopeApp1.dbo.vr_03681d", tables.First());
            Assert.AreEqual(@"ScopeApp1.dbo.Currncy", tables.Last());
        }

    }
}