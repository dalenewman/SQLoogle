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
using System.Text;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class Schema : SQLServerSchemaBase
    {
        public Schema(Database parent)
            : base(parent, Enums.ObjectType.Schema)
        {                        
        }

        public override string ToSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("CREATE SCHEMA ");
            sql.Append("[" + this.Name + "] AUTHORIZATION [" + Owner + "]");
            sql.Append("\r\nGO\r\n");
            return sql.ToString();
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP SCHEMA [" + Name + "]\r\nGO\r\n";
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropSchema);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddSchema);
            }
            return listDiff;
        }
    }
}
