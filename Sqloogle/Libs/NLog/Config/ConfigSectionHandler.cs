#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Configuration;
using System.Xml;
using Sqloogle.Libs.NLog.Common;
using Sqloogle.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Sqloogle.Libs.NLog.Config
{
    /// <summary>
    ///     NLog configuration section handler class for configuring NLog from App.config.
    /// </summary>
    public sealed class ConfigSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        ///     Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                var configFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                return new XmlLoggingConfiguration((XmlElement) section, configFileName);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("ConfigSectionHandler error: {0}", exception);
                throw;
            }
        }
    }
}

#endif