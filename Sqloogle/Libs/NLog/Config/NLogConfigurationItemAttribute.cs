#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Sqloogle.Libs.NLog.Config
{
    /// <summary>
    ///     Marks the object as configuration item for NLog.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NLogConfigurationItemAttribute : Attribute
    {
    }
}