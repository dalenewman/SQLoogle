using System.Configuration;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {

    /// <summary>
    /// Generic input command operation that takes the
    /// actual connection string itself, rather than a
    /// connection string from the configuration, or a
    /// connection string settings object.
    /// </summary>
    public abstract class InputOperation : InputCommandOperation     {

        private const string PROVIDER = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private static string _connectionString;

        protected InputOperation(string connectionString) : base(GetConnectionStringSettings(connectionString))
        {
            UseTransaction = false;
        }

        private static ConnectionStringSettings GetConnectionStringSettings(string connectionString)
        {
            _connectionString = connectionString;
            return new ConnectionStringSettings {
                ConnectionString = _connectionString,
                ProviderName = PROVIDER,
            };
        }

    }
}
