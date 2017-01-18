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
namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class ObjectDependency
    {
        private string columnName;
        private string name;
        private Enums.ObjectType type;

        public ObjectDependency(string name, string Column, Enums.ObjectType type)
        {
            this.name = name;
            this.columnName = Column;
            this.type = type;
        }

        public ObjectDependency(string name, string Column)
        {
            this.name = name;
            this.columnName = Column;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        public Enums.ObjectType Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool IsCodeType
        {
            get { return ((type == Enums.ObjectType.StoreProcedure) || (type == Enums.ObjectType.Trigger) || (type == Enums.ObjectType.View) || (type == Enums.ObjectType.Function)); }

        }
    }
}
