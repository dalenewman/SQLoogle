#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#if !NET_CF && !SILVERLIGHT

namespace Sqloogle.Libs.NLog.ComInterop
{
    /// <summary>
    ///     NLog COM Interop logger interface.
    /// </summary>
    [Guid("757fd55a-cc93-4b53-a7a0-18e85620704a")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [ComVisible(true)]
    public interface IComLogger
    {
        /// <summary>
        ///     Writes the diagnostic message at the specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        void Log(string level, string message);

        /// <summary>
        ///     Writes the diagnostic message at the Trace level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        void Trace(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Debug level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        void Debug(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Info level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        void Info(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Warn level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        void Warn(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Error level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "That's NLog API.")]
        void Error(string message);

        /// <summary>
        ///     Writes the diagnostic message at the Fatal level.
        /// </summary>
        /// <param name="message">
        ///     A <see langword="string" /> to be written.
        /// </param>
        void Fatal(string message);

        /// <summary>
        ///     Checks if the specified log level is enabled.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <returns>A value indicating whether the specified log level is enabled.</returns>
        bool IsEnabled(string level);

        /// <summary>
        ///     Gets a value indicating whether the Trace level is enabled.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "This is COM object and properties cannot be reordered")]
        bool IsTraceEnabled { get; }

        /// <summary>
        ///     Gets a value indicating whether the Debug level is enabled.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "This is COM object and properties cannot be reordered")]
        bool IsDebugEnabled { get; }

        /// <summary>
        ///     Gets a value indicating whether the Info level is enabled.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "This is COM object and properties cannot be reordered")]
        bool IsInfoEnabled { get; }

        /// <summary>
        ///     Gets a value indicating whether the Warn level is enabled.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "This is COM object and properties cannot be reordered")]
        bool IsWarnEnabled { get; }

        /// <summary>
        ///     Gets a value indicating whether the Error level is enabled.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "This is COM object and properties cannot be reordered")]
        bool IsErrorEnabled { get; }

        /// <summary>
        ///     Gets a value indicating whether the Fatal level is enabled.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "This is COM object and properties cannot be reordered")]
        bool IsFatalEnabled { get; }

        /// <summary>
        ///     Gets or sets the logger name.
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "This is COM object and properties cannot be reordered")]
        string LoggerName { get; set; }
    }
}

#endif