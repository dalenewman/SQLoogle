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
using System.Collections.Generic;
using System.Collections;
using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class Rule : Code
    {
        public Rule(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Rule, Enums.ScripActionType.AddRule, Enums.ScripActionType.DropRule)
        {
        }

        public new Rule Clone(ISchemaBase parent)
        {
            Rule item = new Rule(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Text = this.Text;
            item.Guid = this.Guid;
            return item;
        }

        public string ToSQLAddBind()
        {
            string sql;
            if (this.Parent.ObjectType == Enums.ObjectType.Column)
                sql = String.Format("EXEC sp_bindrule N'{0}', N'[{1}].[{2}]','futureonly'\r\nGO\r\n", Name, this.Parent.Parent.Name,this.Parent.Name);
            else
                sql = String.Format("EXEC sp_bindrule N'{0}', N'{1}','futureonly'\r\nGO\r\n", Name, this.Parent.Name);
            return sql;
        }

        public string ToSQLAddUnBind()
        {
            string sql;
            if (this.Parent.ObjectType == Enums.ObjectType.Column)
                sql = String.Format("EXEC sp_unbindrule @objname=N'[{0}].[{1}]'\r\nGO\r\n", this.Parent.Parent.Name, this.Parent.Name);
            else
                sql = String.Format("EXEC sp_unbindrule @objname=N'{0}'\r\nGO\r\n", this.Parent.Name);
            return sql;
        }

        private SQLScriptList ToSQLUnBindAll()
        {
            SQLScriptList listDiff = new SQLScriptList();
            Hashtable items = new Hashtable();
            List<UserDataType> useDataTypes = ((Database)this.Parent).UserTypes.FindAll(item => { return item.Rule.FullName.Equals(this.FullName); });
            foreach (UserDataType item in useDataTypes)
            {
                foreach (ObjectDependency dependency in item.Dependencys)
                {
                    Column column = ((Database)this.Parent).Tables[dependency.Name].Columns[dependency.ColumnName];
                    if ((!column.IsComputed) && (column.Status != Enums.ObjectStatusType.CreateStatus))
                    {
                        if (!items.ContainsKey(column.FullName))
                        {
                            listDiff.Add("EXEC sp_unbindrule '" + column.FullName + "'\r\nGO\r\n", 0, Enums.ScripActionType.UnbindRuleColumn);
                            items.Add(column.FullName, column.FullName);
                        }
                    }
                }
                if (item.Rule.Status != Enums.ObjectStatusType.CreateStatus)
                    listDiff.Add("EXEC sp_unbindrule '" + item.FullName + "'\r\nGO\r\n", 0, Enums.ScripActionType.UnbindRuleType);
            }
            return listDiff;
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.AddRange(ToSQLUnBindAll());
                listDiff.Add(Drop());
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                listDiff.Add(Create());
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.AddRange(ToSQLUnBindAll());
                listDiff.AddRange(Rebuild());
            }
            return listDiff;
        }
    }
}