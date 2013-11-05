#region License
// /*
// See license included in this library folder.
// */
#endregion

using Sqloogle.Libs.NLog.Config;

namespace Sqloogle.Libs.NLog.Internal
{
    /// <summary>
    ///     Supports object initialization and termination.
    /// </summary>
    internal interface ISupportsInitialize
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        void Initialize(LoggingConfiguration configuration);

        /// <summary>
        ///     Closes this instance.
        /// </summary>
        void Close();
    }
}