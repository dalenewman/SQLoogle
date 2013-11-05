#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using Sqloogle.Libs.NLog.Common;
using Sqloogle.Libs.NLog.Internal;
using Sqloogle.Libs.NLog.Layouts;

namespace Sqloogle.Libs.NLog
{
    /// <summary>
    ///     Represents the logging event.
    /// </summary>
    public class LogEventInfo
    {
        /// <summary>
        ///     Gets the date of the first log event created.
        /// </summary>
        public static readonly DateTime ZeroDate = DateTime.UtcNow;

        private static int globalSequenceId;

        private string formattedMessage;
        private IDictionary<Layout, string> layoutCache;
        private IDictionary<object, object> properties;
        private IDictionary eventContextAdapter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        public LogEventInfo()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Logger name.</param>
        /// <param name="message">Log message including parameter placeholders.</param>
        public LogEventInfo(LogLevel level, string loggerName, [Localizable(false)] string message)
            : this(level, loggerName, null, message, null, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Logger name.</param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">Log message including parameter placeholders.</param>
        /// <param name="parameters">Parameter array.</param>
        public LogEventInfo(LogLevel level, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters)
            : this(level, loggerName, formatProvider, message, parameters, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEventInfo" /> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="loggerName">Logger name.</param>
        /// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
        /// <param name="message">Log message including parameter placeholders.</param>
        /// <param name="parameters">Parameter array.</param>
        /// <param name="exception">Exception information.</param>
        public LogEventInfo(LogLevel level, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters, Exception exception)
        {
            TimeStamp = CurrentTimeGetter.Now;
            Level = level;
            LoggerName = loggerName;
            Message = message;
            Parameters = parameters;
            FormatProvider = formatProvider;
            Exception = exception;
            SequenceID = Interlocked.Increment(ref globalSequenceId);

            if (NeedToPreformatMessage(parameters))
            {
                CalcFormattedMessage();
            }
        }

        /// <summary>
        ///     Gets the unique identifier of log event which is automatically generated
        ///     and monotonously increasing.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "Backwards compatibility")]
        public int SequenceID { get; private set; }

        /// <summary>
        ///     Gets or sets the timestamp of the logging event.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TimeStamp", Justification = "Backwards compatibility.")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        ///     Gets or sets the level of the logging event.
        /// </summary>
        public LogLevel Level { get; set; }

#if !NET_CF
        /// <summary>
        ///     Gets a value indicating whether stack trace has been set for this event.
        /// </summary>
        public bool HasStackTrace
        {
            get { return StackTrace != null; }
        }

        /// <summary>
        ///     Gets the stack frame of the method that did the logging.
        /// </summary>
        public StackFrame UserStackFrame
        {
            get { return (StackTrace != null) ? StackTrace.GetFrame(UserStackFrameNumber) : null; }
        }

        /// <summary>
        ///     Gets the number index of the stack frame that represents the user
        ///     code (not the NLog code).
        /// </summary>
        public int UserStackFrameNumber { get; private set; }

        /// <summary>
        ///     Gets the entire stack trace.
        /// </summary>
        public StackTrace StackTrace { get; private set; }
#endif

        /// <summary>
        ///     Gets or sets the exception information.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        ///     Gets or sets the logger name.
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        ///     Gets the logger short name.
        /// </summary>
        [Obsolete("This property should not be used.")]
        public string LoggerShortName
        {
            get
            {
                var lastDot = LoggerName.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    return LoggerName.Substring(lastDot + 1);
                }

                return LoggerName;
            }
        }

        /// <summary>
        ///     Gets or sets the log message including any parameter placeholders.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the parameter values or null if no parameters have been specified.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "For backwards compatibility.")]
        public object[] Parameters { get; set; }

        /// <summary>
        ///     Gets or sets the format provider that was provided while logging or <see langword="null" />
        ///     when no formatProvider was specified.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        ///     Gets the formatted message.
        /// </summary>
        public string FormattedMessage
        {
            get
            {
                if (formattedMessage == null)
                {
                    CalcFormattedMessage();
                }

                return formattedMessage;
            }
        }

        /// <summary>
        ///     Gets the dictionary of per-event context properties.
        /// </summary>
        public IDictionary<object, object> Properties
        {
            get
            {
                if (properties == null)
                {
                    InitEventContext();
                }

                return properties;
            }
        }

        /// <summary>
        ///     Gets the dictionary of per-event context properties.
        /// </summary>
        [Obsolete("Use LogEventInfo.Properties instead.", true)]
        public IDictionary Context
        {
            get
            {
                if (eventContextAdapter == null)
                {
                    InitEventContext();
                }

                return eventContextAdapter;
            }
        }

        /// <summary>
        ///     Creates the null event.
        /// </summary>
        /// <returns>Null log event.</returns>
        public static LogEventInfo CreateNullEvent()
        {
            return new LogEventInfo(LogLevel.Off, string.Empty, string.Empty);
        }

        /// <summary>
        ///     Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     Instance of <see cref="LogEventInfo" />.
        /// </returns>
        public static LogEventInfo Create(LogLevel logLevel, string loggerName, [Localizable(false)] string message)
        {
            return new LogEventInfo(logLevel, loggerName, null, message, null);
        }

        /// <summary>
        ///     Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        ///     Instance of <see cref="LogEventInfo" />.
        /// </returns>
        public static LogEventInfo Create(LogLevel logLevel, string loggerName, IFormatProvider formatProvider, [Localizable(false)] string message, object[] parameters)
        {
            return new LogEventInfo(logLevel, loggerName, formatProvider, message, parameters);
        }

        /// <summary>
        ///     Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        ///     Instance of <see cref="LogEventInfo" />.
        /// </returns>
        public static LogEventInfo Create(LogLevel logLevel, string loggerName, IFormatProvider formatProvider, object message)
        {
            return new LogEventInfo(logLevel, loggerName, formatProvider, "{0}", new[] {message});
        }

        /// <summary>
        ///     Creates the log event.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///     Instance of <see cref="LogEventInfo" />.
        /// </returns>
        public static LogEventInfo Create(LogLevel logLevel, string loggerName, [Localizable(false)] string message, Exception exception)
        {
            return new LogEventInfo(logLevel, loggerName, null, message, null, exception);
        }

        /// <summary>
        ///     Creates <see cref="AsyncLogEventInfo" /> from this <see cref="LogEventInfo" /> by attaching the specified asynchronous continuation.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        /// <returns>
        ///     Instance of <see cref="AsyncLogEventInfo" /> with attached continuation.
        /// </returns>
        public AsyncLogEventInfo WithContinuation(AsyncContinuation asyncContinuation)
        {
            return new AsyncLogEventInfo(this, asyncContinuation);
        }

        /// <summary>
        ///     Returns a string representation of this log event.
        /// </summary>
        /// <returns>String representation of the log event.</returns>
        public override string ToString()
        {
            return "Log Event: Logger='" + LoggerName + "' Level=" + Level + " Message='" + FormattedMessage + "' SequenceID=" + SequenceID;
        }

#if !NET_CF
        /// <summary>
        ///     Sets the stack trace for the event info.
        /// </summary>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="userStackFrame">Index of the first user stack frame within the stack trace.</param>
        public void SetStackTrace(StackTrace stackTrace, int userStackFrame)
        {
            StackTrace = stackTrace;
            UserStackFrameNumber = userStackFrame;
        }
#endif

        internal string AddCachedLayoutValue(Layout layout, string value)
        {
            if (layoutCache == null)
            {
                layoutCache = new Dictionary<Layout, string>();
            }

            lock (layoutCache)
            {
                layoutCache[layout] = value;
            }

            return value;
        }

        internal bool TryGetCachedLayoutValue(Layout layout, out string value)
        {
            if (layoutCache == null)
            {
                value = null;
                return false;
            }

            lock (layoutCache)
            {
                return layoutCache.TryGetValue(layout, out value);
            }
        }

        private static bool NeedToPreformatMessage(object[] parameters)
        {
            // we need to preformat message if it contains any parameters which could possibly
            // do logging in their ToString()
            if (parameters == null || parameters.Length == 0)
            {
                return false;
            }

            if (parameters.Length > 3)
            {
                // too many parameters, too costly to check
                return true;
            }

            if (!IsSafeToDeferFormatting(parameters[0]))
            {
                return true;
            }

            if (parameters.Length >= 2)
            {
                if (!IsSafeToDeferFormatting(parameters[1]))
                {
                    return true;
                }
            }

            if (parameters.Length >= 3)
            {
                if (!IsSafeToDeferFormatting(parameters[2]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSafeToDeferFormatting(object value)
        {
            if (value == null)
            {
                return true;
            }

            return value.GetType().IsPrimitive || (value is string);
        }

        private void CalcFormattedMessage()
        {
            if (Parameters == null || Parameters.Length == 0)
            {
                formattedMessage = Message;
            }
            else
            {
                try
                {
                    formattedMessage = string.Format(FormatProvider ?? CultureInfo.CurrentCulture, Message, Parameters);
                }
                catch (Exception exception)
                {
                    formattedMessage = Message;
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    InternalLogger.Warn("Error when formatting a message: {0}", exception);
                }
            }
        }

        private void InitEventContext()
        {
            properties = new Dictionary<object, object>();
            eventContextAdapter = new DictionaryAdapter<object, object>(properties);
        }
    }
}