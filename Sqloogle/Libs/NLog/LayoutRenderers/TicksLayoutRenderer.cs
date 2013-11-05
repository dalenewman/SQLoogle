#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Globalization;
using System.Text;
using Sqloogle.Libs.NLog.Config;

namespace Sqloogle.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     The Ticks value of current date and time.
    /// </summary>
    [LayoutRenderer("ticks")]
    [ThreadAgnostic]
    public class TicksLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Renders the ticks value of current time and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(logEvent.TimeStamp.Ticks.ToString(CultureInfo.InvariantCulture));
        }
    }
}