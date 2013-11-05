#region License
// /*
// See license included in this library folder.
// */
#endregion

using Sqloogle.Libs.NLog.Config;

namespace Sqloogle.Libs.NLog.Internal
{
    /// <summary>
    ///     Allows components to request stack trace information to be provided in the <see cref="LogEventInfo" />.
    /// </summary>
    internal interface IUsesStackTrace
    {
        /// <summary>
        ///     Gets the level of stack trace information required by the implementing class.
        /// </summary>
        StackTraceUsage StackTraceUsage { get; }
    }
}