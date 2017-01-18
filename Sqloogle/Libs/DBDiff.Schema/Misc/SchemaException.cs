#region license
// Sqloogle
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
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
