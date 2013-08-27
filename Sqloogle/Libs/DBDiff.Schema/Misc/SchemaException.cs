﻿using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Globalization;

namespace Sqloogle.Libs.DBDiff.Schema.Misc
{
    [Serializable]
    public class SchemaException : Exception
    {
        private static void Write(string message)
        {
            try
            {
                StreamWriter writer = new StreamWriter("OpenDBDiff.log", true, Encoding.ASCII);
                writer.WriteLine("ERROR: " + DateTime.Now.ToString("yyyy/MM/dd hh:mm", CultureInfo.InvariantCulture) + "-" + message);
                writer.Close();
            }
            finally { }
        }

        public SchemaException():base()
        {
        }

        public SchemaException(string message)
            : base(message)
        {
            Write(base.StackTrace);
        }

        public SchemaException(string message, Exception exception)
            : base(message, exception)
        {
            Write(exception.StackTrace);
        }

        protected SchemaException(SerializationInfo info, StreamingContext context):base(info, context)
        {
        }
    }
}
