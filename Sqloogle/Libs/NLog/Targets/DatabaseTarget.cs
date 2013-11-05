#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using Sqloogle.Libs.NLog.Common;
using Sqloogle.Libs.NLog.Config;
using Sqloogle.Libs.NLog.Internal;
using Sqloogle.Libs.NLog.Layouts;

#if !SILVERLIGHT

namespace Sqloogle.Libs.NLog.Targets
{
    /// <summary>
    ///     Writes log messages to the database using an ADO.NET provider.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Database_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <para>
    ///         The configuration is dependent on the database type, because
    ///         there are differnet methods of specifying connection string, SQL
    ///         command and command parameters.
    ///     </para>
    ///     <para>MS SQL Server using System.Data.SqlClient:</para>
    ///     <code lang="XML" source="examples/targets/Configuration File/Database/MSSQL/NLog.config" height="450" />
    ///     <para>Oracle using System.Data.OracleClient:</para>
    ///     <code lang="XML" source="examples/targets/Configuration File/Database/Oracle.Native/NLog.config" height="350" />
    ///     <para>Oracle using System.Data.OleDBClient:</para>
    ///     <code lang="XML" source="examples/targets/Configuration File/Database/Oracle.OleDB/NLog.config" height="350" />
    ///     <para>To set up the log target programmatically use code like this (an equivalent of MSSQL configuration):</para>
    ///     <code lang="C#" source="examples/targets/Configuration API/Database/MSSQL/Example.cs" height="630" />
    /// </example>
    [Target("Database")]
    public sealed class DatabaseTarget : Target, IInstallable
    {
        private static readonly Assembly systemDataAssembly = typeof (IDbConnection).Assembly;

        private IDbConnection activeConnection;
        private string activeConnectionString;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseTarget" /> class.
        /// </summary>
        public DatabaseTarget()
        {
            Parameters = new List<DatabaseParameterInfo>();
            InstallDdlCommands = new List<DatabaseCommandInfo>();
            UninstallDdlCommands = new List<DatabaseCommandInfo>();
            DBProvider = "sqlserver";
            DBHost = ".";
#if !NET_CF
            ConnectionStringsSettings = ConfigurationManager.ConnectionStrings;
#endif
        }

        /// <summary>
        ///     Gets or sets the name of the database provider.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The parameter name should be a provider invariant name as registered in machine.config or app.config. Common values are:
        ///     </para>
        ///     <ul>
        ///         <li>
        ///             <c>System.Data.SqlClient</c> -
        ///             <see
        ///                 href="http://msdn.microsoft.com/en-us/library/system.data.sqlclient.aspx">
        ///                 SQL Sever Client
        ///             </see>
        ///         </li>
        ///         <li>
        ///             <c>System.Data.SqlServerCe.3.5</c> - <see href="http://www.microsoft.com/sqlserver/2005/en/us/compact.aspx">SQL Sever Compact 3.5</see>
        ///         </li>
        ///         <li>
        ///             <c>System.Data.OracleClient</c> -
        ///             <see
        ///                 href="http://msdn.microsoft.com/en-us/library/system.data.oracleclient.aspx">
        ///                 Oracle Client from Microsoft
        ///             </see>
        ///             (deprecated in .NET Framework 4)
        ///         </li>
        ///         <li>
        ///             <c>Oracle.DataAccess.Client</c> -
        ///             <see
        ///                 href="http://www.oracle.com/technology/tech/windows/odpnet/index.html">
        ///                 ODP.NET provider from Oracle
        ///             </see>
        ///         </li>
        ///         <li>
        ///             <c>System.Data.SQLite</c> - <see href="http://sqlite.phxsoftware.com/">System.Data.SQLite driver for SQLite</see>
        ///         </li>
        ///         <li>
        ///             <c>Npgsql</c> - <see href="http://npgsql.projects.postgresql.org/">Npgsql driver for PostgreSQL</see>
        ///         </li>
        ///         <li>
        ///             <c>MySql.Data.MySqlClient</c> - <see href="http://www.mysql.com/downloads/connector/net/">MySQL Connector/Net</see>
        ///         </li>
        ///     </ul>
        ///     <para>(Note that provider invariant names are not supported on .NET Compact Framework).</para>
        ///     <para>
        ///         Alternatively the parameter value can be be a fully qualified name of the provider
        ///         connection type (class implementing <see cref="IDbConnection" />) or one of the following tokens:
        ///     </para>
        ///     <ul>
        ///         <li>
        ///             <c>sqlserver</c>, <c>mssql</c>, <c>microsoft</c> or <c>msde</c> - SQL Server Data Provider
        ///         </li>
        ///         <li>
        ///             <c>oledb</c> - OLEDB Data Provider
        ///         </li>
        ///         <li>
        ///             <c>odbc</c> - ODBC Data Provider
        ///         </li>
        ///     </ul>
        /// </remarks>
        /// <docgen category='Connection Options' order='10' />
        [RequiredParameter]
        [DefaultValue("sqlserver")]
        public string DBProvider { get; set; }

#if !NET_CF
        /// <summary>
        ///     Gets or sets the name of the connection string (as specified in
        ///     <see
        ///         href="http://msdn.microsoft.com/en-us/library/bf7sd233.aspx">
        ///         &lt;connectionStrings&gt; configuration section
        ///     </see>
        ///     .
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        public string ConnectionStringName { get; set; }
#endif

        /// <summary>
        ///     Gets or sets the connection string. When provided, it overrides the values
        ///     specified in DBHost, DBUserName, DBPassword, DBDatabase.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        public Layout ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the connection string using for installation and uninstallation. If not provided, regular ConnectionString is being used.
        /// </summary>
        /// <docgen category='Installation Options' order='10' />
        public Layout InstallConnectionString { get; set; }

        /// <summary>
        ///     Gets the installation DDL commands.
        /// </summary>
        /// <docgen category='Installation Options' order='10' />
        [ArrayParameter(typeof (DatabaseCommandInfo), "install-command")]
        public IList<DatabaseCommandInfo> InstallDdlCommands { get; private set; }

        /// <summary>
        ///     Gets the uninstallation DDL commands.
        /// </summary>
        /// <docgen category='Installation Options' order='10' />
        [ArrayParameter(typeof (DatabaseCommandInfo), "uninstall-command")]
        public IList<DatabaseCommandInfo> UninstallDdlCommands { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to keep the
        ///     database connection open between the log events.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        [DefaultValue(true)]
        public bool KeepConnection { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to use database transactions.
        ///     Some data providers require this.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        [DefaultValue(false)]
        public bool UseTransactions { get; set; }

        /// <summary>
        ///     Gets or sets the database host name. If the ConnectionString is not provided
        ///     this value will be used to construct the "Server=" part of the
        ///     connection string.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        public Layout DBHost { get; set; }

        /// <summary>
        ///     Gets or sets the database user name. If the ConnectionString is not provided
        ///     this value will be used to construct the "User ID=" part of the
        ///     connection string.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        public Layout DBUserName { get; set; }

        /// <summary>
        ///     Gets or sets the database password. If the ConnectionString is not provided
        ///     this value will be used to construct the "Password=" part of the
        ///     connection string.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        public Layout DBPassword { get; set; }

        /// <summary>
        ///     Gets or sets the database name. If the ConnectionString is not provided
        ///     this value will be used to construct the "Database=" part of the
        ///     connection string.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        public Layout DBDatabase { get; set; }

        /// <summary>
        ///     Gets or sets the text of the SQL command to be run on each log level.
        /// </summary>
        /// <remarks>
        ///     Typically this is a SQL INSERT statement or a stored procedure call.
        ///     It should use the database-specific parameters (marked as <c>@parameter</c>
        ///     for SQL server or <c>:parameter</c> for Oracle, other data providers
        ///     have their own notation) and not the layout renderers,
        ///     because the latter is prone to SQL injection attacks.
        ///     The layout renderers should be specified as &lt;parameter /&gt; elements instead.
        /// </remarks>
        /// <docgen category='SQL Statement' order='10' />
        [RequiredParameter]
        public Layout CommandText { get; set; }

        /// <summary>
        ///     Gets the collection of parameters. Each parameter contains a mapping
        ///     between NLog layout and a database named or positional parameter.
        /// </summary>
        /// <docgen category='SQL Statement' order='11' />
        [ArrayParameter(typeof (DatabaseParameterInfo), "parameter")]
        public IList<DatabaseParameterInfo> Parameters { get; private set; }

#if !NET_CF
        internal DbProviderFactory ProviderFactory { get; set; }

        // this is so we can mock the connection string without creating sub-processes
        internal ConnectionStringSettingsCollection ConnectionStringsSettings { get; set; }
#endif

        internal Type ConnectionType { get; set; }

        /// <summary>
        ///     Performs installation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        public void Install(InstallationContext installationContext)
        {
            RunInstallCommands(installationContext, InstallDdlCommands);
        }

        /// <summary>
        ///     Performs uninstallation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        public void Uninstall(InstallationContext installationContext)
        {
            RunInstallCommands(installationContext, UninstallDdlCommands);
        }

        /// <summary>
        ///     Determines whether the item is installed.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        /// <returns>
        ///     Value indicating whether the item is installed or null if it is not possible to determine.
        /// </returns>
        public bool? IsInstalled(InstallationContext installationContext)
        {
            return null;
        }

        internal IDbConnection OpenConnection(string connectionString)
        {
            IDbConnection connection;

#if !NET_CF
            if (ProviderFactory != null)
            {
                connection = ProviderFactory.CreateConnection();
            }
            else
#endif
            {
                connection = (IDbConnection) Activator.CreateInstance(ConnectionType);
            }

            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }

        /// <summary>
        ///     Initializes the target. Can be used by inheriting classes
        ///     to initialize logging.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "connectionStrings", Justification = "Name of the config file section.")]
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            var foundProvider = false;

#if !NET_CF
            if (!string.IsNullOrEmpty(ConnectionStringName))
            {
                // read connection string and provider factory from the configuration file
                var cs = ConnectionStringsSettings[ConnectionStringName];
                if (cs == null)
                {
                    throw new NLogConfigurationException("Connection string '" + ConnectionStringName + "' is not declared in <connectionStrings /> section.");
                }

                ConnectionString = SimpleLayout.Escape(cs.ConnectionString);
                ProviderFactory = DbProviderFactories.GetFactory(cs.ProviderName);
                foundProvider = true;
            }

            if (!foundProvider)
            {
                foreach (DataRow row in DbProviderFactories.GetFactoryClasses().Rows)
                {
                    if ((string) row["InvariantName"] == DBProvider)
                    {
                        ProviderFactory = DbProviderFactories.GetFactory(DBProvider);
                        foundProvider = true;
                    }
                }
            }
#endif

            if (!foundProvider)
            {
                switch (DBProvider.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "SQLSERVER":
                    case "MSSQL":
                    case "MICROSOFT":
                    case "MSDE":
                        ConnectionType = systemDataAssembly.GetType("System.Data.SqlClient.SqlConnection", true);
                        break;

                    case "OLEDB":
                        ConnectionType = systemDataAssembly.GetType("System.Data.OleDb.OleDbConnection", true);
                        break;

                    case "ODBC":
                        ConnectionType = systemDataAssembly.GetType("System.Data.Odbc.OdbcConnection", true);
                        break;

                    default:
                        ConnectionType = Type.GetType(DBProvider, true);
                        break;
                }
            }
        }

        /// <summary>
        ///     Closes the target and releases any unmanaged resources.
        /// </summary>
        protected override void CloseTarget()
        {
            base.CloseTarget();

            CloseConnection();
        }

        /// <summary>
        ///     Writes the specified logging event to the database. It creates
        ///     a new database command, prepares parameters for it by calculating
        ///     layouts and executes the command.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            try
            {
                WriteEventToDatabase(logEvent);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Error when writing to database {0}", exception);
                CloseConnection();
                throw;
            }
            finally
            {
                if (!KeepConnection)
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>
        ///     Writes an array of logging events to the log target. By default it iterates on all
        ///     events and passes them to "Write" method. Inheriting classes can use this method to
        ///     optimize batch writes.
        /// </summary>
        /// <param name="logEvents">Logging events to be written out.</param>
        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            var buckets = logEvents.BucketSort(c => BuildConnectionString(c.LogEvent));

            try
            {
                foreach (var kvp in buckets)
                {
                    foreach (var ev in kvp.Value)
                    {
                        try
                        {
                            WriteEventToDatabase(ev.LogEvent);
                            ev.Continuation(null);
                        }
                        catch (Exception exception)
                        {
                            if (exception.MustBeRethrown())
                            {
                                throw;
                            }

                            // in case of exception, close the connection and report it
                            InternalLogger.Error("Error when writing to database {0}", exception);
                            CloseConnection();
                            ev.Continuation(exception);
                        }
                    }
                }
            }
            finally
            {
                if (!KeepConnection)
                {
                    CloseConnection();
                }
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "It's up to the user to ensure proper quoting.")]
        private void WriteEventToDatabase(LogEventInfo logEvent)
        {
            EnsureConnectionOpen(BuildConnectionString(logEvent));

            var command = activeConnection.CreateCommand();
            command.CommandText = CommandText.Render(logEvent);

            InternalLogger.Trace("Executing {0}: {1}", command.CommandType, command.CommandText);

            foreach (var par in Parameters)
            {
                var p = command.CreateParameter();
                p.Direction = ParameterDirection.Input;
                if (par.Name != null)
                {
                    p.ParameterName = par.Name;
                }

                if (par.Size != 0)
                {
                    p.Size = par.Size;
                }

                if (par.Precision != 0)
                {
                    p.Precision = par.Precision;
                }

                if (par.Scale != 0)
                {
                    p.Scale = par.Scale;
                }

                var stringValue = par.Layout.Render(logEvent);

                p.Value = stringValue;
                command.Parameters.Add(p);

                InternalLogger.Trace("  Parameter: '{0}' = '{1}' ({2})", p.ParameterName, p.Value, p.DbType);
            }

            var result = command.ExecuteNonQuery();
            InternalLogger.Trace("Finished execution, result = {0}", result);
        }

        private string BuildConnectionString(LogEventInfo logEvent)
        {
            if (ConnectionString != null)
            {
                return ConnectionString.Render(logEvent);
            }

            var sb = new StringBuilder();

            sb.Append("Server=");
            sb.Append(DBHost.Render(logEvent));
            sb.Append(";");
            if (DBUserName == null)
            {
                sb.Append("Trusted_Connection=SSPI;");
            }
            else
            {
                sb.Append("User id=");
                sb.Append(DBUserName.Render(logEvent));
                sb.Append(";Password=");
                sb.Append(DBPassword.Render(logEvent));
                sb.Append(";");
            }

            if (DBDatabase != null)
            {
                sb.Append("Database=");
                sb.Append(DBDatabase.Render(logEvent));
            }

            return sb.ToString();
        }

        private void EnsureConnectionOpen(string connectionString)
        {
            if (activeConnection != null)
            {
                if (activeConnectionString != connectionString)
                {
                    CloseConnection();
                }
            }

            if (activeConnection != null)
            {
                return;
            }

            activeConnection = OpenConnection(connectionString);
            activeConnectionString = connectionString;
        }

        private void CloseConnection()
        {
            if (activeConnection != null)
            {
                activeConnection.Close();
                activeConnection = null;
                activeConnectionString = null;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "It's up to the user to ensure proper quoting.")]
        private void RunInstallCommands(InstallationContext installationContext, IEnumerable<DatabaseCommandInfo> commands)
        {
            // create log event that will be used to render all layouts
            var logEvent = installationContext.CreateLogEvent();

            try
            {
                foreach (var commandInfo in commands)
                {
                    string cs;

                    if (commandInfo.ConnectionString != null)
                    {
                        // if there is connection string specified on the command info, use it
                        cs = commandInfo.ConnectionString.Render(logEvent);
                    }
                    else if (InstallConnectionString != null)
                    {
                        // next, try InstallConnectionString
                        cs = InstallConnectionString.Render(logEvent);
                    }
                    else
                    {
                        // if it's not defined, fall back to regular connection string
                        cs = BuildConnectionString(logEvent);
                    }

                    EnsureConnectionOpen(cs);

                    var command = activeConnection.CreateCommand();
                    command.CommandType = commandInfo.CommandType;
                    command.CommandText = commandInfo.Text.Render(logEvent);

                    try
                    {
                        installationContext.Trace("Executing {0} '{1}'", command.CommandType, command.CommandText);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        if (commandInfo.IgnoreFailures || installationContext.IgnoreFailures)
                        {
                            installationContext.Warning(exception.Message);
                        }
                        else
                        {
                            installationContext.Error(exception.Message);
                            throw;
                        }
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

#endif