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

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class Default:SQLServerSchemaBase 
    {
        private string value;

        public Default(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Default)
        {
        }

        public new Default Clone(ISchemaBase parent)
        {
            Default item = new Default(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Value = this.Value;
            return item;
        }
        
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public string ToSQLAddBind()
        {
            string sql = "";
            sql += "EXEC sp_bindefault N'" + Name + "', N'" + this.Parent.Name + "'\r\nGO\r\n";
            return sql;
        }

        public string ToSQLAddUnBind()
        {
            string sql = "";
            sql += "EXEC sp_unbindefault @objname=N'" + this.Parent.Name + "'\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override string ToSqlDrop()
        {
            return "DROP DEFAULT " + FullName + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return "";
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropRule);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddRule);
            }
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropRule);
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddRule);
            }
            return listDiff;
        }
    }
}
