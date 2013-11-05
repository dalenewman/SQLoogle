#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using Sqloogle.Libs.NLog.Config;
using Sqloogle.Libs.NLog.LayoutRenderers;
using Sqloogle.Libs.NLog.Layouts;

namespace Sqloogle.Libs.NLog.Targets
{
    /// <summary>
    ///     Sends log messages to the remote instance of NLog Viewer.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/NLogViewer_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/NLogViewer/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/NLogViewer/Simple/Example.cs" />
    ///     <p>
    ///         NOTE: If your receiver application is ever likely to be off-line, don't use TCP protocol
    ///         or you'll get TCP timeouts and your application will crawl.
    ///         Either switch to UDP transport or use <a href="target.AsyncWrapper.html">AsyncWrapper</a> target
    ///         so that your application threads will not be blocked by the timing-out connection attempts.
    ///     </p>
    /// </example>
    [Target("NLogViewer")]
    public class NLogViewerTarget : NetworkTarget
    {
        private readonly Log4JXmlEventLayout layout = new Log4JXmlEventLayout();

        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogViewerTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public NLogViewerTarget()
        {
            Parameters = new List<NLogViewerParameterInfo>();
            Renderer.Parameters = Parameters;
            NewLine = false;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to include NLog-specific extensions to log4j schema.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeNLogData
        {
            get { return Renderer.IncludeNLogData; }
            set { Renderer.IncludeNLogData = value; }
        }

        /// <summary>
        ///     Gets or sets the AppInfo field. By default it's the friendly name of the current AppDomain.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public string AppInfo
        {
            get { return Renderer.AppInfo; }
            set { Renderer.AppInfo = value; }
        }

#if !NET_CF
        /// <summary>
        ///     Gets or sets a value indicating whether to include call site (class and method name) in the information sent over the network.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeCallSite
        {
            get { return Renderer.IncludeCallSite; }
            set { Renderer.IncludeCallSite = value; }
        }

#if !SILVERLIGHT
        /// <summary>
        ///     Gets or sets a value indicating whether to include source info (file name and line number) in the information sent over the network.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeSourceInfo
        {
            get { return Renderer.IncludeSourceInfo; }
            set { Renderer.IncludeSourceInfo = value; }
        }
#endif

#endif

        /// <summary>
        ///     Gets or sets a value indicating whether to include <see cref="MappedDiagnosticsContext" /> dictionary contents.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeMdc
        {
            get { return Renderer.IncludeMdc; }
            set { Renderer.IncludeMdc = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to include <see cref="NestedDiagnosticsContext" /> stack contents.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeNdc
        {
            get { return Renderer.IncludeNdc; }
            set { Renderer.IncludeNdc = value; }
        }

        /// <summary>
        ///     Gets or sets the NDC item separator.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public string NdcItemSeparator
        {
            get { return Renderer.NdcItemSeparator; }
            set { Renderer.NdcItemSeparator = value; }
        }

        /// <summary>
        ///     Gets the collection of parameters. Each parameter contains a mapping
        ///     between NLog layout and a named parameter.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        [ArrayParameter(typeof (NLogViewerParameterInfo), "parameter")]
        public IList<NLogViewerParameterInfo> Parameters { get; private set; }

        /// <summary>
        ///     Gets the layout renderer which produces Log4j-compatible XML events.
        /// </summary>
        public Log4JXmlEventLayoutRenderer Renderer
        {
            get { return layout.Renderer; }
        }

        /// <summary>
        ///     Gets or sets the instance of <see cref="Log4JXmlEventLayout" /> that is used to format log messages.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public override Layout Layout
        {
            get { return layout; }

            set { }
        }
    }
}