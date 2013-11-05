#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Sqloogle.Libs.NLog.Config
{
    /// <summary>
    ///     Attribute used to mark the default parameters for layout renderers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DefaultParameterAttribute : Attribute
    {
    }
}