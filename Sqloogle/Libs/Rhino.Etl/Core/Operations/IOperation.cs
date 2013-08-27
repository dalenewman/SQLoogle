using System;
using System.Collections.Generic;

namespace Sqloogle.Libs.Rhino.Etl.Core.Operations
{
    /// <summary>
    /// A single operation in an etl process
    /// </summary>
    public interface IOperation : IDisposable
    {         
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Sets the transaction.
        /// </summary>
        /// <value>True or false.</value>
        bool UseTransaction { get; set; }

        /// <summary>
        /// Gets the statistics for this operation
        /// </summary>
        /// <value>The statistics.</value>
        OperationStatistics Statistics { get; }

        /// <summary>
        /// Occurs when a row is processed.
        /// </summary>
        event Action<IOperation, Row> OnRowProcessed;

        /// <summary>
        /// Occurs when all the rows has finished processing.
        /// </summary>
        event Action<IOperation> OnFinishedProcessing;

        /// <summary>
        /// Initializes the current instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        void PrepareForExecution(IPipelineExecuter pipelineExecuter);

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        IEnumerable<Row> Execute(IEnumerable<Row> rows);

        /// <summary>
        /// Raises the row processed event
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        void RaiseRowProcessed(Row dictionary);

        /// <summary>
        /// Raises the finished processing event
        /// </summary>
        void RaiseFinishedProcessing();

        /// <summary>
        /// Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exception> GetAllErrors();
    }

}
