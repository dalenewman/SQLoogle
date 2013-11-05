#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Sqloogle.Libs.NLog.Common;
using Sqloogle.Libs.NLog.Config;
using Sqloogle.Libs.NLog.Internal;

namespace Sqloogle.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Exception information provided through
    ///     a call to one of the Logger.*Exception() methods.
    /// </summary>
    [LayoutRenderer("exception")]
    [ThreadAgnostic]
    public class ExceptionLayoutRenderer : LayoutRenderer
    {
        private string format;
        private string innerFormat = string.Empty;
        private ExceptionDataTarget[] exceptionDataTargets;
        private ExceptionDataTarget[] innerExceptionDataTargets;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExceptionLayoutRenderer" /> class.
        /// </summary>
        public ExceptionLayoutRenderer()
        {
            Format = "message";
            Separator = " ";
            InnerExceptionSeparator = EnvironmentHelper.NewLine;
            MaxInnerExceptionLevel = 0;
        }

        private delegate void ExceptionDataTarget(StringBuilder sb, Exception ex);

        /// <summary>
        ///     Gets or sets the format of the output. Must be a comma-separated list of exception
        ///     properties: Message, Type, ShortType, ToString, Method, StackTrace.
        ///     This parameter value is case-insensitive.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Format
        {
            get { return format; }

            set
            {
                format = value;
                exceptionDataTargets = CompileFormat(value);
            }
        }

        /// <summary>
        ///     Gets or sets the format of the output of inner exceptions. Must be a comma-separated list of exception
        ///     properties: Message, Type, ShortType, ToString, Method, StackTrace.
        ///     This parameter value is case-insensitive.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string InnerFormat
        {
            get { return innerFormat; }

            set
            {
                innerFormat = value;
                innerExceptionDataTargets = CompileFormat(value);
            }
        }

        /// <summary>
        ///     Gets or sets the separator used to concatenate parts specified in the Format.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(" ")]
        public string Separator { get; set; }

        /// <summary>
        ///     Gets or sets the maximum number of inner exceptions to include in the output.
        ///     By default inner exceptions are not enabled for compatibility with NLog 1.0.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(0)]
        public int MaxInnerExceptionLevel { get; set; }

        /// <summary>
        ///     Gets or sets the separator between inner exceptions.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string InnerExceptionSeparator { get; set; }

        /// <summary>
        ///     Renders the specified exception information and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.Exception != null)
            {
                var sb2 = new StringBuilder(128);
                var separator = string.Empty;

                foreach (var targetRenderFunc in exceptionDataTargets)
                {
                    sb2.Append(separator);
                    targetRenderFunc(sb2, logEvent.Exception);
                    separator = Separator;
                }

                var currentException = logEvent.Exception.InnerException;
                var currentLevel = 0;
                while (currentException != null && currentLevel < MaxInnerExceptionLevel)
                {
                    // separate inner exceptions
                    sb2.Append(InnerExceptionSeparator);

                    separator = string.Empty;
                    foreach (var targetRenderFunc in innerExceptionDataTargets ?? exceptionDataTargets)
                    {
                        sb2.Append(separator);
                        targetRenderFunc(sb2, currentException);
                        separator = Separator;
                    }

                    currentException = currentException.InnerException;
                    currentLevel++;
                }

                builder.Append(sb2);
            }
        }

        private static void AppendMessage(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.Message);
        }

        private static void AppendMethod(StringBuilder sb, Exception ex)
        {
#if SILVERLIGHT || NET_CF
            sb.Append(ParseMethodNameFromStackTrace(ex.StackTrace));
#else
            if (ex.TargetSite != null)
            {
                sb.Append(ex.TargetSite);
            }
#endif
        }

        private static void AppendStackTrace(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.StackTrace);
        }

        private static void AppendToString(StringBuilder sb, Exception ex)
        {
            sb.Append(ex);
        }

        private static void AppendType(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.GetType().FullName);
        }

        private static void AppendShortType(StringBuilder sb, Exception ex)
        {
            sb.Append(ex.GetType().Name);
        }

        private static ExceptionDataTarget[] CompileFormat(string formatSpecifier)
        {
            var parts = formatSpecifier.Replace(" ", string.Empty).Split(',');
            var dataTargets = new List<ExceptionDataTarget>();

            foreach (var s in parts)
            {
                switch (s.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "MESSAGE":
                        dataTargets.Add(AppendMessage);
                        break;

                    case "TYPE":
                        dataTargets.Add(AppendType);
                        break;

                    case "SHORTTYPE":
                        dataTargets.Add(AppendShortType);
                        break;

                    case "TOSTRING":
                        dataTargets.Add(AppendToString);
                        break;

                    case "METHOD":
                        dataTargets.Add(AppendMethod);
                        break;

                    case "STACKTRACE":
                        dataTargets.Add(AppendStackTrace);
                        break;

                    default:
                        InternalLogger.Warn("Unknown exception data target: {0}", s);
                        break;
                }
            }

            return dataTargets.ToArray();
        }

#if SILVERLIGHT || NET_CF
        private static string ParseMethodNameFromStackTrace(string stackTrace)
        {
            // get the first line of the stack trace
            string stackFrameLine;

            int p = stackTrace.IndexOfAny(new[] { '\r', '\n' });
            if (p >= 0)
            {
                stackFrameLine = stackTrace.Substring(0, p);
            }
            else
            {
                stackFrameLine = stackTrace;
            }

            // stack trace is composed of lines which look like this
            //
            // at NLog.UnitTests.LayoutRenderers.ExceptionTests.GenericClass`3.Method2[T1,T2,T3](T1 aaa, T2 b, T3 o, Int32 i, DateTime now, Nullable`1 gfff, List`1[] something)
            //
            // "at " prefix can be localized so we cannot hard-code it but it's followed by a space, class name (which does not have a space in it) and opening paranthesis
            int lastSpace = -1;
            int startPos = 0;
            int endPos = stackFrameLine.Length;

            for (int i = 0; i < stackFrameLine.Length; ++i)
            {
                switch (stackFrameLine[i])
                {
                    case ' ':
                        lastSpace = i;
                        break;

                    case '(':
                        startPos = lastSpace + 1;
                        break;

                    case ')':
                        endPos = i + 1;

                        // end the loop
                        i = stackFrameLine.Length;
                        break;
                }
            }

            return stackTrace.Substring(startPos, endPos - startPos);
        }
#endif
    }
}