#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Sqloogle.Libs.NLog.Conditions
{
    /// <summary>
    ///     Marks the class as containing condition methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ConditionMethodsAttribute : Attribute
    {
    }
}