#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Sqloogle.Libs.NLog.LayoutRenderers;
using Sqloogle.Libs.NLog.Layouts;
using Sqloogle.Libs.NLog.Targets;

namespace Sqloogle.Libs.NLog.Config
{
    /// <summary>
    ///     Attaches a simple name to an item (such as <see cref="Target" />,
    ///     <see cref="LayoutRenderer" />, <see cref="Layout" />, etc.).
    /// </summary>
    public abstract class NameBaseAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NameBaseAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        protected NameBaseAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     Gets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        public string Name { get; private set; }
    }
}