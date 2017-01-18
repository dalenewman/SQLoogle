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
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Interfaces;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public abstract class SQLServerSchemaBase:SchemaBase, ISQLServerSchemaBase
    {
        private SchemaList<ExtendedProperty,ISchemaBase> extendedProperties;

        protected SQLServerSchemaBase(ISchemaBase parent, Enums.ObjectType objectType):base("[", "]", objectType)
        {
            this.Parent = parent;
            extendedProperties = new SchemaList<ExtendedProperty, ISchemaBase>(parent);
        }

        public SchemaList<ExtendedProperty, ISchemaBase> ExtendedProperties
        {
            get { return extendedProperties; }
        }
    }
}
