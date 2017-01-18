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
    public class Synonym : SQLServerSchemaBase
    {
        private string value;

        public Synonym(ISchemaBase parent)
            : base(parent, Enums.ObjectType.Synonym)
        {
        }

        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Synonym item = new Synonym(parent);
            item.Id = this.Id;
            item.Name = this.Name;
            item.Owner = this.Owner;
            item.Value = this.Value;
            item.Guid = this.Guid;
            return item;
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public override string ToSql()
        {
            string sql = "CREATE SYNONYM " + FullName + " FOR " + value + "\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP SYNONYM " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropSynonyms);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddSynonyms);
            }
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropSynonyms);
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddSynonyms);
            }
            return listDiff;
        }

        /// <summary>
        /// Compara dos Synonyms y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public static Boolean Compare(Synonym origen, Synonym destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.Value.Equals(destino.Value)) return false;
            return true;
        }
    }
}
