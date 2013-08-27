using System.Collections.Generic;
using NUnit.Framework;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Tests.Integration
{

    public class EtlProcessHelper
    {
        
        protected List<Row> TestOperation(params IOperation[] operations)
        {
            return new TestProcess(operations).ExecuteWithResults();
        }

        protected class TestProcess : EtlProcess
        {
            List<Row> returnRows = new List<Row>();

            private class ResultsOperation : AbstractOperation
            {
                public ResultsOperation(List<Row> returnRows)
                {
                    this.returnRows = returnRows;
                }

                List<Row> returnRows = null;

                public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
                {
                    returnRows.AddRange(rows);

                    return rows;
                }
            }

            public TestProcess(params IOperation[] testOperations)
            {
                this.testOperations = testOperations;
            }

            IEnumerable<IOperation> testOperations = null;

            protected override void Initialize()
            {
                foreach (var testOperation in testOperations)
                    Register(testOperation);

                Register(new ResultsOperation(returnRows));
            }

            public List<Row> ExecuteWithResults()
            {
                Execute();
                return returnRows;
            }
        }


    }

}
