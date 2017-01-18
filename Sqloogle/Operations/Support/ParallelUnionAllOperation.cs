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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {

    /// <summary>
    /// Combines rows from all operations.
    /// </summary>
    public class ParallelUnionAllOperation : AbstractOperation {

        private readonly List<IOperation> _operations = new List<IOperation>();

        public ParallelUnionAllOperation() {
            UseTransaction = false;
        }

        public ParallelUnionAllOperation(IEnumerable<IOperation> ops) {
            _operations.AddRange(ops);
        }

        public ParallelUnionAllOperation(params IOperation[] ops) {
            _operations.AddRange(ops);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var blockingCollection = new BlockingCollection<Row>();
            var count = _operations.Count;
            if (count == 0) {
                yield break;
            }

            Debug("Creating tasks for {0} operations.", count);

            var tasks = _operations.Select(currentOp =>
            Task.Factory.StartNew(() => {
                try {
                    foreach (var row in currentOp.Execute(null)) {
                        blockingCollection.Add(row);
                    }
                } finally {
                    if (Interlocked.Decrement(ref count) == 0) {
                        blockingCollection.CompleteAdding();
                    }
                }
            })).ToArray();

            foreach (var row in blockingCollection.GetConsumingEnumerable()) {
                yield return row;
            }
            Task.WaitAll(tasks); // raise any exception that were raised during execution
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

        /// <summary>
        /// Add operation parameters
        /// </summary>
        /// <param name="ops">operations delimited by commas</param>
        /// <returns></returns>
        public ParallelUnionAllOperation Add(params IOperation[] ops) {
            _operations.AddRange(ops);
            return this;
        }

        /// <summary>
        /// Add operations
        /// </summary>
        /// <param name="ops">an enumerable of operations</param>
        /// <returns></returns>
        public ParallelUnionAllOperation Add(IEnumerable<IOperation> ops) {
            _operations.AddRange(ops);
            return this;
        }

    }
}
