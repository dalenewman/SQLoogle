#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sqloogle.Libs.NLog.Common;
using Sqloogle.Libs.NLog.Internal;
using Sqloogle.Libs.NLog.Targets;

namespace Sqloogle.Libs.NLog.Config
{
    /// <summary>
    ///     Keeps logging configuration and provides simple API
    ///     to modify it.
    /// </summary>
    public class LoggingConfiguration
    {
        private readonly IDictionary<string, Target> targets =
            new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);

        private object[] configItems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingConfiguration" /> class.
        /// </summary>
        public LoggingConfiguration()
        {
            LoggingRules = new List<LoggingRule>();
        }

        /// <summary>
        ///     Gets a collection of named targets specified in the configuration.
        /// </summary>
        /// <returns>
        ///     A list of named targets.
        /// </returns>
        /// <remarks>
        ///     Unnamed targets (such as those wrapped by other targets) are not returned.
        /// </remarks>
        public ReadOnlyCollection<Target> ConfiguredNamedTargets
        {
            get { return new List<Target>(targets.Values).AsReadOnly(); }
        }

        /// <summary>
        ///     Gets the collection of file names which should be watched for changes by NLog.
        /// </summary>
        public virtual IEnumerable<string> FileNamesToWatch
        {
            get { return new string[0]; }
        }

        /// <summary>
        ///     Gets the collection of logging rules.
        /// </summary>
        public IList<LoggingRule> LoggingRules { get; private set; }

        /// <summary>
        ///     Gets all targets.
        /// </summary>
        public ReadOnlyCollection<Target> AllTargets
        {
            get { return configItems.OfType<Target>().ToList().AsReadOnly(); }
        }

        /// <summary>
        ///     Registers the specified target object under a given name.
        /// </summary>
        /// <param name="name">
        ///     Name of the target.
        /// </param>
        /// <param name="target">
        ///     The target object.
        /// </param>
        public void AddTarget(string name, Target target)
        {
            if (name == null)
            {
                throw new ArgumentException("Target name cannot be null", "name");
            }

            InternalLogger.Debug("Registering target {0}: {1}", name, target.GetType().FullName);
            targets[name] = target;
        }

        /// <summary>
        ///     Finds the target with the specified name.
        /// </summary>
        /// <param name="name">
        ///     The name of the target to be found.
        /// </param>
        /// <returns>
        ///     Found target or <see langword="null" /> when the target is not found.
        /// </returns>
        public Target FindTargetByName(string name)
        {
            Target value;

            if (!targets.TryGetValue(name, out value))
            {
                return null;
            }

            return value;
        }

        /// <summary>
        ///     Called by LogManager when one of the log configuration files changes.
        /// </summary>
        /// <returns>
        ///     A new instance of <see cref="LoggingConfiguration" /> that represents the updated configuration.
        /// </returns>
        public virtual LoggingConfiguration Reload()
        {
            return this;
        }

        /// <summary>
        ///     Removes the specified named target.
        /// </summary>
        /// <param name="name">
        ///     Name of the target.
        /// </param>
        public void RemoveTarget(string name)
        {
            targets.Remove(name);
        }

        /// <summary>
        ///     Installs target-specific objects on current system.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        /// <remarks>
        ///     Installation typically runs with administrative permissions.
        /// </remarks>
        public void Install(InstallationContext installationContext)
        {
            if (installationContext == null)
            {
                throw new ArgumentNullException("installationContext");
            }

            InitializeAll();
            foreach (var installable in configItems.OfType<IInstallable>())
            {
                installationContext.Info("Installing '{0}'", installable);

                try
                {
                    installable.Install(installationContext);
                    installationContext.Info("Finished installing '{0}'.", installable);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    installationContext.Error("'{0}' installation failed: {1}.", installable, exception);
                }
            }
        }

        /// <summary>
        ///     Uninstalls target-specific objects from current system.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        /// <remarks>
        ///     Uninstallation typically runs with administrative permissions.
        /// </remarks>
        public void Uninstall(InstallationContext installationContext)
        {
            if (installationContext == null)
            {
                throw new ArgumentNullException("installationContext");
            }

            InitializeAll();

            foreach (var installable in configItems.OfType<IInstallable>())
            {
                installationContext.Info("Uninstalling '{0}'", installable);

                try
                {
                    installable.Uninstall(installationContext);
                    installationContext.Info("Finished uninstalling '{0}'.", installable);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    installationContext.Error("Uninstallation of '{0}' failed: {1}.", installable, exception);
                }
            }
        }

        /// <summary>
        ///     Closes all targets and releases any unmanaged resources.
        /// </summary>
        internal void Close()
        {
            InternalLogger.Debug("Closing logging configuration...");
            foreach (var initialize in configItems.OfType<ISupportsInitialize>())
            {
                InternalLogger.Trace("Closing {0}", initialize);
                try
                {
                    initialize.Close();
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    InternalLogger.Warn("Exception while closing {0}", exception);
                }
            }

            InternalLogger.Debug("Finished closing logging configuration.");
        }

        internal void Dump()
        {
            InternalLogger.Debug("--- NLog configuration dump. ---");
            InternalLogger.Debug("Targets:");
            foreach (var target in targets.Values)
            {
                InternalLogger.Info("{0}", target);
            }

            InternalLogger.Debug("Rules:");
            foreach (var rule in LoggingRules)
            {
                InternalLogger.Info("{0}", rule);
            }

            InternalLogger.Debug("--- End of NLog configuration dump ---");
        }

        /// <summary>
        ///     Flushes any pending log messages on all appenders.
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        internal void FlushAllTargets(AsyncContinuation asyncContinuation)
        {
            var uniqueTargets = new List<Target>();
            foreach (var rule in LoggingRules)
            {
                foreach (var t in rule.Targets)
                {
                    if (!uniqueTargets.Contains(t))
                    {
                        uniqueTargets.Add(t);
                    }
                }
            }

            AsyncHelpers.ForEachItemInParallel(uniqueTargets, asyncContinuation, (target, cont) => target.Flush(cont));
        }

        /// <summary>
        ///     Validates the configuration.
        /// </summary>
        internal void ValidateConfig()
        {
            var roots = new List<object>();
            foreach (var r in LoggingRules)
            {
                roots.Add(r);
            }

            foreach (var target in targets.Values)
            {
                roots.Add(target);
            }

            configItems = ObjectGraphScanner.FindReachableObjects<object>(roots.ToArray());

            // initialize all config items starting from most nested first
            // so that whenever the container is initialized its children have already been
            InternalLogger.Info("Found {0} configuration items", configItems.Length);

            foreach (var o in configItems)
            {
                PropertyHelper.CheckRequiredParameters(o);
            }
        }

        internal void InitializeAll()
        {
            ValidateConfig();

            foreach (var initialize in configItems.OfType<ISupportsInitialize>().Reverse())
            {
                InternalLogger.Trace("Initializing {0}", initialize);

                try
                {
                    initialize.Initialize(this);
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }

                    if (LogManager.ThrowExceptions)
                    {
                        throw new NLogConfigurationException("Error during initialization of " + initialize, exception);
                    }
                }
            }
        }

        internal void EnsureInitialized()
        {
            InitializeAll();
        }
    }
}