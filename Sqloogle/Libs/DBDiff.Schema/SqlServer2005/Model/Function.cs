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
    public class Function : Code 
    {
        public Function(ISchemaBase parent)
            : base(parent,Enums.ObjectType.Function, Enums.ScripActionType.AddFunction, Enums.ScripActionType.DropFunction)
        {

        }

        /// <summary>
        /// Clona el objeto en una nueva instancia.
        /// </summary>
        public override ISchemaBase Clone(ISchemaBase parent)
        {
            Function item = new Function(parent);
            item.Text = this.Text;
            item.Status = this.Status;
            item.Name = this.Name;
            item.Id = this.Id;
            item.Owner = this.Owner;
            item.Guid = this.Guid;
            item.IsSchemaBinding = this.IsSchemaBinding;
            this.DependenciesIn.ForEach(dep => item.DependenciesIn.Add(dep));
            this.DependenciesOut.ForEach(dep => item.DependenciesOut.Add(dep));
            return item;
        }

        public override Boolean IsCodeType
        {
            get { return true; }
        }

        public string ToSQLAlter()
        {
            return ToSQLAlter(false);
        }

        public string ToSQLAlter(Boolean quitSchemaBinding)
        {
            return FormatCode.FormatAlter("FUNCTION", ToSql(), this, quitSchemaBinding);
        }

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

                if (!this.GetWasInsertInDiffList(Enums.ScripActionType.DropFunction))
                {
                    if (this.HasState(Enums.ObjectStatusType.RebuildStatus))
                    {
                        list.Add(Drop());
                        list.Add(Create());
                    }
                    if (this.HasState(Enums.ObjectStatusType.AlterBodyStatus))
                    {
                        int iCount = DependenciesCount;
                        list.Add(ToSQLAlter(), iCount, Enums.ScripActionType.AlterFunction);
                    }
                }
            }
            return list;
        }
    }
}
