using System;
using NUnit.Framework;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Operations;
using Sqloogle.Processes;

namespace Tests.Integration {
    [TestFixture]
    public class RunSqloogleProcess : EtlProcessHelper {

        [Test]
        public void Run() {
            var process = new SqloogleProcess();
            process.Execute();
            process.ReportErrors();
        }

        [Test]
        public void UpdateDocument() {
            const string sql = @"-- uspPrintError prints error information about the error that caused 
-- execution to jump to the CATCH block of a TRY...CATCH construct. 
-- Should be executed from within the scope of a CATCH block otherwise 
-- it will return without printing any error information.
CREATE PROCEDURE [dbo].[uspPrintError] 
AS
BEGIN
    SET NOCOUNT ON;

    -- Print error information. 
    PRINT 'Error ' + CONVERT(varchar(50), ERROR_NUMBER()) +
          ', Severity ' + CONVERT(varchar(5), ERROR_SEVERITY()) +
          ', State ' + CONVERT(varchar(5), ERROR_STATE()) + 
          ', Procedure ' + ISNULL(ERROR_PROCEDURE(), '-') + 
          ', Line ' + CONVERT(varchar(5), ERROR_LINE());
    PRINT ERROR_MESSAGE();
END;
GO
";
            const string yesterday = "20130429";
            var testData = new FakeOperation(
               new Row { { "action", "Update" }, { "count", "0000000001" }, { "created", "20060426" }, { "database", "AdventureWorks" }, { "dropped", "False" }, { "id", "X1785396043" }, { "lastused", yesterday }, { "modified", "20060426" }, { "name", "uspPrintError" }, { "schema", "dbo" }, { "server", "localhost" }, { "sqlscript", sql }, { "type", "Stored Procedure" }, { "use", "0000000000" } }
            );
            var luceneLoad = new LuceneLoad(@"c:\Sqloogle\SearchIndex");
            var results = TestOperation(testData, luceneLoad);
        }

    }
}
