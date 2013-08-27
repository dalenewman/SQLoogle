using System.Configuration;
using System.Data;

namespace Sqloogle.Libs.Rhino.Etl.Core.Operations
{
    /// <summary>
    /// Base class for operations that directly manipulate ADO.Net
    /// It is important to remember that this is supposed to be a deep base class, not to be 
    /// directly inherited or used
    /// </summary>
    public abstract class AbstractCommandOperation : AbstractDatabaseOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDatabaseOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        protected AbstractCommandOperation(string connectionStringName)
            : this(ConfigurationManager.ConnectionStrings[connectionStringName])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDatabaseOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">The connection string settings to use.</param>
        protected AbstractCommandOperation(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings)
        {
        }

        /// <summary>
        /// The current command
        /// </summary>
        protected IDbCommand currentCommand;

        /// <summary>
        /// Adds the parameter to the current command
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected void AddParameter(string name, object value)
        {
            AddParameter(currentCommand, name, value);
        }

        /// <summary>
        /// Begins a transaction conditionally based on the UseTransaction property
        /// </summary>
        /// <param name="connection">The IDbConnection object you are working with</param>
        /// <returns>An open IDbTransaction object or null.</returns>
        protected IDbTransaction BeginTransaction(IDbConnection connection)
        {
            if (UseTransaction)
            {
                return connection.BeginTransaction();
            }

            return null;
        }
    }
}