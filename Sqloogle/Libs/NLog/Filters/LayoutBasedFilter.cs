#region License
// /*
// See license included in this library folder.
// */
#endregion

using Sqloogle.Libs.NLog.Config;
using Sqloogle.Libs.NLog.Layouts;

namespace Sqloogle.Libs.NLog.Filters
{
    /// <summary>
    ///     A base class for filters that are based on comparing a value to a layout.
    /// </summary>
    public abstract class LayoutBasedFilter : Filter
    {
        /// <summary>
        ///     Gets or sets the layout to be used to filter log messages.
        /// </summary>
        /// <value>The layout.</value>
        /// <docgen category='Filtering Options' order='10' />
        [RequiredParameter]
        public Layout Layout { get; set; }
    }
}