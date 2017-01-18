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
using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class TableOption : SQLServerSchemaBase
    {
        private string vale;

        public TableOption(string Name, string value, ISchemaBase parent)
            : base(parent, Enums.ObjectType.TableOption)
        {
            this.Name = Name;
            this.Value = value;
        }

        public TableOption(ISchemaBase parent)
            : base(parent, Enums.ObjectType.TableOption)
        {
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            TableOption option = new TableOption(parent);
            option.Name = this.Name;
            option.Status = this.Status;
            option.Value = this.Value;
            return option;
        }

        public string Value
        {
            get { return vale; }
            set { vale = value; }
        }

                /// <summary>
        /// Compara dos indices y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(TableOption origen, TableOption destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!destino.Value.Equals(origen.Value)) return false;
            return true;
        }

        public override string ToSqlDrop()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row','off'\r\nGO\r\n";
            if (this.Name.Equals("LargeValues"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'large value types out of row','0'\r\nGO\r\n";
            if (this.Name.Equals("VarDecimal"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'vardecimal storage format','0'\r\nGO\r\n";
            if (this.Name.Equals("LockEscalation"))
                return "";
            return "";
        }

        public override string ToSql()
        {
            if (this.Name.Equals("TextInRow"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'text in row'," + vale + "\r\nGO\r\n";
            if (this.Name.Equals("LargeValues"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'large value types out of row'," + vale + "\r\nGO\r\n";
            if (this.Name.Equals("VarDecimal"))
                return "EXEC sp_tableoption " + Parent.Name + ", 'vardecimal storage format','1'\r\nGO\r\n";
            if (this.Name.Equals("LockEscalation"))
            {
                if ((!this.Value.Equals("TABLE")) || (this.Status != Enums.ObjectStatusType.OriginalStatus))
                    return "ALTER TABLE " + Parent.Name + " SET (LOCK_ESCALATION = " + Value + ")\r\nGO\r\n";
            }
            return "";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.AddOptions);
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.DropOptions);
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropOptions);
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddOptions);
            }
            return listDiff;
        }
    }
}
