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
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Util;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class StoreProcedure : Code 
    {
        public StoreProcedure(ISchemaBase parent)
            : base(parent, Enums.ObjectType.StoreProcedure, Enums.ScripActionType.AddStoreProcedure, Enums.ScripActionType.DropStoreProcedure)
        {

        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            StoreProcedure item = new StoreProcedure(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            return item;
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public override string ToSql()
        {
            //if (String.IsNullOrEmpty(sql))
                sql = FormatCode.FormatCreate("PROC(EDURE)?", Text, this);
            return sql;
        }

        public string ToSQLAlter()
        {
            return FormatCode.FormatAlter("PROC(EDURE)?", ToSql(), this, false);
        }

        /// <summary>
        /// Devuelve el schema de diferencias del Schema en formato SQL.
        /// </summary>
        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();
            if (this.Status != Enums.ObjectStatusType.OriginalStatus)
                RootParent.ActionMessage.Add(this);

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
                list.Add(ToSQLAlter(), 0, Enums.ScripActionType.AlterProcedure);
            return list;
        }
    }
}
