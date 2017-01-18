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

namespace Sqloogle.Operations.Support {
    public class SerialUnionAllOperation : AbstractOperation {

        private readonly List<IOperation> _operations = new List<IOperation>();

        public SerialUnionAllOperation() {
            UseTransaction = false;
        }

        public SerialUnionAllOperation(IEnumerable<IOperation> ops) {
            _operations.AddRange(ops);
        }

        public SerialUnionAllOperation(params IOperation[] ops) {
            _operations.AddRange(ops);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var operation in _operations)
                foreach (var row in operation.Execute(null))
                    yield return row;
        }

        public SerialUnionAllOperation Add(params IOperation[] operation) {
            _operations.AddRange(operation);
            return this;
        }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter) {
            foreach (var operation in _operations) {
                operation.PrepareForExecution(pipelineExecuter);
            }
        }

    }
}