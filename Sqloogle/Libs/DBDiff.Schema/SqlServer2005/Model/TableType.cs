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
using NLog;
using Sqloogle.Libs.DBDiff.Schema.Model;
namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model {
    public class TableType : SQLServerSchemaBase, ITable<TableType> {
        private readonly Logger _logger = LogManager.GetLogger("DbDiff");
        private Columns<TableType> columns;
        private SchemaList<Constraint, TableType> constraints;
        private SchemaList<Index, TableType> indexes;

        public TableType(Database parent)
            : base(parent, Enums.ObjectType.TableType) {
            columns = new Columns<TableType>(this);
            constraints = new SchemaList<Constraint, TableType>(this, parent.AllObjects);
            indexes = new SchemaList<Index, TableType>(this, parent.AllObjects);
        }

        public Columns<TableType> Columns
        {
            get { return columns; }
        }

        public SchemaList<Constraint, TableType> Constraints
        {
            get { return constraints; }
        }

        public SchemaList<Index, TableType> Indexes
        {
            get { return indexes; }
        }

        public override string ToSql() {
            string sql = "";
            if (columns.Count > 0) {
                sql += "CREATE TYPE " + FullName + " AS TABLE\r\n(\r\n";
                sql += columns.ToSql() + "\r\n";
                sql += constraints.ToSql();
                sql += ")";
                sql += "\r\nGO\r\n";
            }
            return sql;
        }

        public override string ToSqlDrop() {
            return "DROP TYPE " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd() {
            return ToSql();
        }

        public override SQLScript Create() {
            Enums.ScripActionType action = Enums.ScripActionType.AddTableType;
            if (!GetWasInsertInDiffList(action)) {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlAdd(), 0, action);
            } else
                return null;
        }

        public override SQLScript Drop() {
            Enums.ScripActionType action = Enums.ScripActionType.DropTableType;
            if (!GetWasInsertInDiffList(action)) {
                SetWasInsertInDiffList(action);
                return new SQLScript(this.ToSqlDrop(), 0, action);
            } else
                return null;
        }

        public override SQLScriptList ToSqlDiff() {
            try {
                SQLScriptList list = new SQLScriptList();
                if (this.Status == Enums.ObjectStatusType.DropStatus) {
                    list.Add(Drop());
                }
                if (this.HasState(Enums.ObjectStatusType.CreateStatus)) {
                    list.Add(Create());
                }
                if (this.Status == Enums.ObjectStatusType.AlterStatus) {
                    list.Add(ToSqlDrop() + ToSql(), 0, Enums.ScripActionType.AddTableType);
                }
                return list;
            } catch (Exception ex) {
                _logger.Error(ex, ex.Message);
                return null;
            }
        }
    }
}
