using System;
using System.Runtime.Serialization;

namespace Sqloogle.Libs.Rhino.Etl.Core.Exceptions
{
    /// <summary>
    /// An exception that was caught during exceuting the code.
    /// </summary>
    [Serializable]
    public class RhinoEtlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RhinoEtlException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RhinoEtlException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhinoEtlException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected RhinoEtlException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}