#region license
// Sqloogle
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Tests {

    public class EtlProcessHelper {
        protected List<Row> TestOperation(params IOperation[] operations) {
            return new TestProcess(operations).ExecuteWithResults();
        }

        protected class TestProcess : EtlProcess {
            List<Row> returnRows = new List<Row>();

            private class ResultsOperation : AbstractOperation {
                public ResultsOperation(List<Row> returnRows) {
                    this.returnRows = returnRows;
                }

                List<Row> returnRows = null;

                public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
                    returnRows.AddRange(rows);

                    return rows;
                }
            }

            public TestProcess(params IOperation[] testOperations) {
                this.testOperations = testOperations;
            }

            IEnumerable<IOperation> testOperations = null;

            protected override void Initialize() {
                foreach (var testOperation in testOperations)
                    Register(testOperation);

                Register(new ResultsOperation(returnRows));
            }

            public List<Row> ExecuteWithResults() {
                Execute();
                return returnRows;
            }
        }


    }

}
