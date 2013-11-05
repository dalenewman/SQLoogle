#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.ComponentModel;
using Sqloogle.Libs.NLog.Internal;

namespace Sqloogle.Libs.NLog
{
    /// <summary>
    ///     Provides logging interface and utility functions.
    /// </summary>
    public class Logger
    {
        private readonly Type loggerType = typeof (Logger);

        private volatile LoggerConfiguration configuration;
        private volatile bool isDebugEnabled;
        private volatile bool isErrorEnabled;
        private volatile bool isFatalEnabled;
        private volatile bool isInfoEnabled;
        private volatile bool isTraceEnabled;
        private volatile bool isWarnEnabled;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Logger" /> class.
        /// </summary>
        protected internal Logger()
        {
        }

        /// <summary>
        ///     Gets the name of the logger.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the factory that created this logger.
        /// </summary>
        public LogFactory Factory { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <c>Trace</c> level.
        /// </summary>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is enabled for the <c>Trace</c> level, otherwise it returns
        ///     <see
        ///         langword="false" />
        ///     .
        /// </returns>
        public bool IsTraceEnabled
        {
            get { return isTraceEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <c>Debug</c> level.
        /// </summary>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is enabled for the <c>Debug</c> level, otherwise it returns
        ///     <see
        ///         langword="false" />
        ///     .
        /// </returns>
        public bool IsDebugEnabled
        {
            get { return isDebugEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <c>Info</c> level.
        /// </summary>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is enabled for the <c>Info</c> level, otherwise it returns
        ///     <see
        ///         langword="false" />
        ///     .
        /// </returns>
        public bool IsInfoEnabled
        {
            get { return isInfoEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <c>Warn</c> level.
        /// </summary>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is enabled for the <c>Warn</c> level, otherwise it returns
        ///     <see
        ///         langword="false" />
        ///     .
        /// </returns>
        public bool IsWarnEnabled
        {
            get { return isWarnEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <c>Error</c> level.
        /// </summary>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is enabled for the <c>Error</c> level, otherwise it returns
        ///     <see
        ///         langword="false" />
        ///     .
        /// </returns>
        public bool IsErrorEnabled
        {
            get { return isErrorEnabled; }
        }

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the <c>Fatal</c> level.
        /// </summary>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is enabled for the <c>Fatal</c> level, otherwise it returns
        ///     <see
        ///         langword="false" />
        ///     .
        /// </returns>
        public bool IsFatalEnabled
        {
            get { return isFatalEnabled; }
        }

        /// <summary>
        ///     Occurs when logger configuration changes.
        /// </summary>
        public event EventHandler<EventArgs> LoggerReconfigured;

        /// <summary>
        ///     Gets a value indicating whether logging is enabled for the specified level.
        /// </summary>
        /// <param name="level">Log level to be checked.</param>
        /// <returns>
        ///     A value of <see langword="true" /> if logging is enabled for the specified level, otherwise it returns
        ///     <see
        ///         langword="false" />
        ///     .
        /// </returns>
        public bool IsEnabled(LogLevel level)
        {
            return GetTargetsForLevel(level) != null;
        }

        /// <summary>
        ///     Writes the specified diagnostic message.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        public void Log(LogEventInfo logEvent)
        {
            if (IsEnabled(logEvent.Level))
            {
                WriteToTargets(logEvent);
            }
        }

        /// <summary>
        ///     Writes the specified diagnostic message.
        /// </summary>
        /// <param name="wrapperType">The name of the type that wraps Logger.</param>
        /// <param name="logEvent">Log event.</param>
        public void Log(Type wrapperType, LogEventInfo logEvent)
        {
            if (IsEnabled(logEvent.Level))
            {
                WriteToTargets(wrapperType, logEvent);
            }
        }

        // the following code has been automatically generated by a PERL script

        // end of generated code
        internal void Initialize(string name, LoggerConfiguration loggerConfiguration, LogFactory factory)
        {
            Name = name;
            Factory = factory;
            SetConfiguration(loggerConfiguration);
        }

        internal void WriteToTargets(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, object[] args)
        {
            LoggerImpl.Write(loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, formatProvider, message, args), Factory);
        }

        internal void WriteToTargets<T>(LogLevel level, IFormatProvider formatProvider, T value)
        {
            LoggerImpl.Write(loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, formatProvider, value), Factory);
        }

        internal void WriteToTargets(LogLevel level, [Localizable(false)] string message, Exception ex)
        {
            LoggerImpl.Write(loggerType, GetTargetsForLevel(level), LogEventInfo.Create(level, Name, message, ex), Factory);
        }

        internal void WriteToTargets(LogLevel level, [Localizable(false)] string message, object[] args)
        {
            WriteToTargets(level, null, message, args);
        }

        internal void WriteToTargets(LogEventInfo logEvent)
        {
            LoggerImpl.Write(loggerType, GetTargetsForLevel(logEvent.Level), logEvent, Factory);
        }

        internal void WriteToTargets(Type wrapperType, LogEventInfo logEvent)
        {
            LoggerImpl.Write(wrapperType, GetTargetsForLevel(logEvent.Level), logEvent, Factory);
        }

        internal void SetConfiguration(LoggerConfiguration newConfiguration)
        {
            configuration = newConfiguration;

            // pre-calculate 'enabled' flags
            isTraceEnabled = newConfiguration.IsEnabled(LogLevel.Trace);
            isDebugEnabled = newConfiguration.IsEnabled(LogLevel.Debug);
            isInfoEnabled = newConfiguration.IsEnabled(LogLevel.Info);
            isWarnEnabled = newConfiguration.IsEnabled(LogLevel.Warn);
            isErrorEnabled = newConfiguration.IsEnabled(LogLevel.Error);
            isFatalEnabled = newConfiguration.IsEnabled(LogLevel.Fatal);

            var loggerReconfiguredDelegate = LoggerReconfigured;

            if (loggerReconfiguredDelegate != null)
            {
                loggerReconfiguredDelegate(this, new EventArgs());
            }
        }

        private TargetWithFilterChain GetTargetsForLevel(LogLevel level)
        {
            return configuration.GetTargetsForLevel(level);
        }

        #region Log() overloads 

        /// <overloads>
        ///     Writes the diagnostic message at the specified level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        ///     Writes the diagnostic message at the specified level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="value">The value to be written.</param>
        public void Log<T>(LogLevel level, T value)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, null, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Log<T>(LogLevel level, IFormatProvider formatProvider, T value)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
        public void Log(LogLevel level, LogMessageGenerator messageFunc)
        {
            if (IsEnabled(level))
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(level, null, messageFunc());
            }
        }

        /// <summary>
        ///     Writes the diagnostic message and exception at the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        /// <param name="exception">An exception to be logged.</param>
        public void LogException(LogLevel level, [Localizable(false)] string message, Exception exception)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, exception);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified parameters and formatting them with the supplied format provider.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Log(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">Log message.</param>
        public void Log(LogLevel level, [Localizable(false)] string message)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, null, message);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified parameters.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Log(LogLevel level, [Localizable(false)] string message, params object[] args)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Log<TArgument>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Log<TArgument>(LogLevel level, [Localizable(false)] string message, TArgument argument)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Log<TArgument1, TArgument2>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Log<TArgument1, TArgument2>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, formatProvider, message, new object[] {argument1, argument2, argument3});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the specified level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsEnabled(level))
            {
                WriteToTargets(level, message, new object[] {argument1, argument2, argument3});
            }
        }

        #endregion

        #region Trace() overloads 

        /// <overloads>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Trace<T>(T value)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, null, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Trace<T>(IFormatProvider formatProvider, T value)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level.
        /// </summary>
        /// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
        public void Trace(LogMessageGenerator messageFunc)
        {
            if (IsTraceEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Trace, null, messageFunc());
            }
        }

        /// <summary>
        ///     Writes the diagnostic message and exception at the <c>Trace</c> level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        /// <param name="exception">An exception to be logged.</param>
        public void TraceException([Localizable(false)] string message, Exception exception)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, exception);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified parameters and formatting them with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Trace(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Trace([Localizable(false)] string message)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, null, message);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Trace([Localizable(false)] string message, params object[] args)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Trace<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Trace<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Trace<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Trace<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Trace<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, formatProvider, message, new object[] {argument1, argument2, argument3});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Trace<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsTraceEnabled)
            {
                WriteToTargets(LogLevel.Trace, message, new object[] {argument1, argument2, argument3});
            }
        }

        #endregion

        #region Debug() overloads 

        /// <overloads>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Debug<T>(T value)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, null, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Debug<T>(IFormatProvider formatProvider, T value)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
        public void Debug(LogMessageGenerator messageFunc)
        {
            if (IsDebugEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Debug, null, messageFunc());
            }
        }

        /// <summary>
        ///     Writes the diagnostic message and exception at the <c>Debug</c> level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        /// <param name="exception">An exception to be logged.</param>
        public void DebugException([Localizable(false)] string message, Exception exception)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, exception);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified parameters and formatting them with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Debug(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Debug([Localizable(false)] string message)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, null, message);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Debug([Localizable(false)] string message, params object[] args)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Debug<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Debug<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Debug<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Debug<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, formatProvider, message, new object[] {argument1, argument2, argument3});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsDebugEnabled)
            {
                WriteToTargets(LogLevel.Debug, message, new object[] {argument1, argument2, argument3});
            }
        }

        #endregion

        #region Info() overloads 

        /// <overloads>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Info<T>(T value)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, null, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Info<T>(IFormatProvider formatProvider, T value)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
        public void Info(LogMessageGenerator messageFunc)
        {
            if (IsInfoEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Info, null, messageFunc());
            }
        }

        /// <summary>
        ///     Writes the diagnostic message and exception at the <c>Info</c> level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        /// <param name="exception">An exception to be logged.</param>
        public void InfoException([Localizable(false)] string message, Exception exception)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, exception);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified parameters and formatting them with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Info(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Info([Localizable(false)] string message)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, null, message);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Info([Localizable(false)] string message, params object[] args)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Info<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Info<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Info<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Info<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, formatProvider, message, new object[] {argument1, argument2, argument3});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Info<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsInfoEnabled)
            {
                WriteToTargets(LogLevel.Info, message, new object[] {argument1, argument2, argument3});
            }
        }

        #endregion

        #region Warn() overloads 

        /// <overloads>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Warn<T>(T value)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, null, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Warn<T>(IFormatProvider formatProvider, T value)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
        public void Warn(LogMessageGenerator messageFunc)
        {
            if (IsWarnEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Warn, null, messageFunc());
            }
        }

        /// <summary>
        ///     Writes the diagnostic message and exception at the <c>Warn</c> level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        /// <param name="exception">An exception to be logged.</param>
        public void WarnException([Localizable(false)] string message, Exception exception)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, exception);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified parameters and formatting them with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Warn(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Warn([Localizable(false)] string message)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, null, message);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Warn([Localizable(false)] string message, params object[] args)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Warn<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Warn<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Warn<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Warn<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, formatProvider, message, new object[] {argument1, argument2, argument3});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsWarnEnabled)
            {
                WriteToTargets(LogLevel.Warn, message, new object[] {argument1, argument2, argument3});
            }
        }

        #endregion

        #region Error() overloads 

        /// <overloads>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Error<T>(T value)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, null, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Error<T>(IFormatProvider formatProvider, T value)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
        public void Error(LogMessageGenerator messageFunc)
        {
            if (IsErrorEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Error, null, messageFunc());
            }
        }

        /// <summary>
        ///     Writes the diagnostic message and exception at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        /// <param name="exception">An exception to be logged.</param>
        public void ErrorException([Localizable(false)] string message, Exception exception)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, exception);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified parameters and formatting them with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Error(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Error([Localizable(false)] string message)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, null, message);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Error([Localizable(false)] string message, params object[] args)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Error<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Error<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Error<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Error<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, formatProvider, message, new object[] {argument1, argument2, argument3});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Error<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsErrorEnabled)
            {
                WriteToTargets(LogLevel.Error, message, new object[] {argument1, argument2, argument3});
            }
        }

        #endregion

        #region Fatal() overloads 

        /// <overloads>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified format provider and format parameters.
        /// </overloads>
        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to be written.</param>
        public void Fatal<T>(T value)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, null, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="value">The value to be written.</param>
        public void Fatal<T>(IFormatProvider formatProvider, T value)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, value);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level.
        /// </summary>
        /// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
        public void Fatal(LogMessageGenerator messageFunc)
        {
            if (IsFatalEnabled)
            {
                if (messageFunc == null)
                {
                    throw new ArgumentNullException("messageFunc");
                }

                WriteToTargets(LogLevel.Fatal, null, messageFunc());
            }
        }

        /// <summary>
        ///     Writes the diagnostic message and exception at the <c>Fatal</c> level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        /// <param name="exception">An exception to be logged.</param>
        public void FatalException([Localizable(false)] string message, Exception exception)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, exception);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters and formatting them with the supplied format provider.
        /// </summary>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Fatal(IFormatProvider formatProvider, [Localizable(false)] string message, params object[] args)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Fatal([Localizable(false)] string message)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, null, message);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> containing format items.
        /// </param>
        /// <param name="args">Arguments to format.</param>
        public void Fatal([Localizable(false)] string message, params object[] args)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, args);
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified parameter and formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Fatal<TArgument>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument argument)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified parameter.
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument">The argument to format.</param>
        public void Fatal<TArgument>([Localizable(false)] string message, TArgument argument)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, new object[] {argument});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Fatal<TArgument1, TArgument2>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        public void Fatal<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, new object[] {argument1, argument2});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified arguments formatting it with the supplied format provider.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider formatProvider, [Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[] {argument1, argument2, argument3});
            }
        }

        /// <summary>
        ///     Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
        /// </summary>
        /// <typeparam name="TArgument1">The type of the first argument.</typeparam>
        /// <typeparam name="TArgument2">The type of the second argument.</typeparam>
        /// <typeparam name="TArgument3">The type of the third argument.</typeparam>
        /// <param name="message">
        ///     A <see langword="string" /> containing one format item.
        /// </param>
        /// <param name="argument1">The first argument to format.</param>
        /// <param name="argument2">The second argument to format.</param>
        /// <param name="argument3">The third argument to format.</param>
        public void Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3)
        {
            if (IsFatalEnabled)
            {
                WriteToTargets(LogLevel.Fatal, message, new object[] {argument1, argument2, argument3});
            }
        }

        #endregion
    }
}