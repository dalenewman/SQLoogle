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
using System.Globalization;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class Parameter
    {
        private string type;
        private int size;
        private string name;
        private byte scale;
        private byte precision;
        private bool output;

        public bool Output
        {
            get { return output; }
            set { output = value; }
        }

        public byte Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public byte Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string ToSql()
        {
            string sql = name + " [" + type + "]";
            if (Type.Equals("binary") || Type.Equals("varbinary") || Type.Equals("varchar") || Type.Equals("char") || Type.Equals("nchar") || Type.Equals("nvarchar"))
            {
                if (Size == -1)
                    sql += "(max)";
                else
                {
                    sql += "(" + Size.ToString(CultureInfo.InvariantCulture) + ")";
                }
            }
            if (type.Equals("numeric") || type.Equals("decimal")) sql += "(" + Precision.ToString(CultureInfo.InvariantCulture) + "," + Scale.ToString(CultureInfo.InvariantCulture) + ")";            
            if (output) sql += " OUTPUT";
            return sql;
        }
    }
}
