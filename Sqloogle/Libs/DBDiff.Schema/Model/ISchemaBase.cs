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

namespace Sqloogle.Libs.DBDiff.Schema.Model
{
    public interface ISchemaBase
    {
        ISchemaBase Clone(ISchemaBase parent);
        int DependenciesCount { get; }
        string FullName { get; }
        int Id { get; set; }
        Boolean HasState(Enums.ObjectStatusType statusFind);
        string Name { get; set; }
        string Owner { get; set; }
        ISchemaBase Parent { get; set;}
        Enums.ObjectStatusType Status { get; set;}                                             
        Boolean IsSystem { get; set; }
        Enums.ObjectType ObjectType { get; set;}
        Boolean GetWasInsertInDiffList(Enums.ScripActionType action);
        void SetWasInsertInDiffList(Enums.ScripActionType action);
        void ResetWasInsertInDiffList();
        string ToSqlDrop();
        string ToSqlAdd();
        string ToSql();
        SQLScriptList ToSqlDiff();
        SQLScript Create();
        SQLScript Drop();
        int CompareFullNameTo(string name, string myName);
        Boolean IsCodeType { get; }
        IDatabase RootParent { get; }
        DateTime ModifyDate { get; set; }
        DateTime CreateDate { get; set; }
    }
}
