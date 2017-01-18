﻿#region license
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
using Sqloogle.Libs.DBDiff.Schema.Attributes;
using Sqloogle.Libs.DBDiff.Schema.Model;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model.Util;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class View : Code 
    {
        private SchemaList<Index, View> indexes;
        private SchemaList<Trigger, View> triggers;
        private SchemaList<CLRTrigger, View> clrtriggers;

        public View(ISchemaBase parent)
            : base(parent, Enums.ObjectType.View, Enums.ScripActionType.AddView, Enums.ScripActionType.DropView)
        {
            indexes = new SchemaList<Index, View>(this, ((Database)parent).AllObjects);
            triggers = new SchemaList<Trigger, View>(this, ((Database)parent).AllObjects);
            clrtriggers = new SchemaList<CLRTrigger, View>(this, ((Database)parent).AllObjects);
        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            View item = new View(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.IsSchemaBinding = this.IsSchemaBinding;
            item.DependenciesIn  = this.DependenciesIn;
            item.DependenciesOut = this.DependenciesOut;
            item.Indexes = this.Indexes.Clone(item);
            item.Triggers = this.Triggers.Clone(item);
            return item;
        }

        [ShowItem("CLR Triggers")]
        public SchemaList<CLRTrigger, View> CLRTriggers
        {
            get { return clrtriggers; }
            set { clrtriggers = value; }
        }

        [ShowItem("Triggers")]
        public SchemaList<Trigger, View> Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }

        [ShowItem("Indexes", "Index")]
        public SchemaList<Index, View> Indexes
        {
            get { return indexes; }
            set { indexes = value; }
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public override string ToSqlAdd()
        {
            string sql = ToSql();
            this.Indexes.ForEach(item =>
                {
                    if (item.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        item.SetWasInsertInDiffList(Enums.ScripActionType.AddIndex); 
                        sql += item.ToSql();
                    }
                }
            );
            this.Triggers.ForEach(item =>
                {
                    if (item.Status != Enums.ObjectStatusType.DropStatus)
                    {
                        item.SetWasInsertInDiffList(Enums.ScripActionType.AddTrigger); 
                        sql += item.ToSql();
                    }
                }
            );

            sql += this.ExtendedProperties.ToSql();
            return sql;
        }

        public string ToSQLAlter()
        {
            return ToSQLAlter(false);
        }

        public string ToSQLAlter(Boolean quitSchemaBinding)
        {
            return FormatCode.FormatAlter("VIEW", ToSql(), this, quitSchemaBinding);
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

            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                if (this.HasState(Enums.ObjectStatusType.RebuildDependenciesStatus))
                    list.AddRange(RebuildDependencys());
                if (this.HasState(Enums.ObjectStatusType.RebuildStatus))
                {
                    list.Add(Drop());
                    list.Add(Create());
                }
                if (this.HasState(Enums.ObjectStatusType.AlterBodyStatus))
                {
                    int iCount = DependenciesCount;
                    list.Add(ToSQLAlter(), iCount, Enums.ScripActionType.AlterView);                    
                }
                if (!this.GetWasInsertInDiffList(Enums.ScripActionType.DropFunction) && (!this.GetWasInsertInDiffList(Enums.ScripActionType.AddFunction)))
                    list.AddRange(indexes.ToSqlDiff());

                list.AddRange(triggers.ToSqlDiff());
            }
            return list;
        }
    }
}
