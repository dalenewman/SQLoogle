using System.Collections.Generic;
using Sqloogle.Libs.Rhino.Etl.Core.Enumerables;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Libs.Rhino.Etl.Core.Pipelines
{
    /// <summary>
    /// Execute all the actions syncronously without caching
    /// </summary>
    public class SingleThreadedNonCachedPipelineExecuter : AbstractPipelineExecuter
    {
        /// <summary>
        /// Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<Row> DecorateEnumerableForExecution(IOperation operation, IEnumerable<Row> enumerator)
        {
            foreach (Row row in new EventRaisingEnumerator(operation, enumerator))
            {
                yield return row;
            }
        }
    }
}