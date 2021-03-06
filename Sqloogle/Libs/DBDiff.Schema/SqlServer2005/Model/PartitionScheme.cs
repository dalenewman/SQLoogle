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
using System.Collections.Generic;
using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class PartitionScheme:SQLServerSchemaBase
    {
        private string partitionFunction;
        private List<string> fileGroups;

        public PartitionScheme(ISchemaBase parent)
            : base(parent, Enums.ObjectType.PartitionFunction)
        {
            fileGroups = new List<string>();
        }

        public List<string> FileGroups
        {
            get { return fileGroups; }
            set { fileGroups = value; }
        } 

        public string PartitionFunction
        {
            get { return partitionFunction; }
            set { partitionFunction = value; }
        }

        public override string ToSqlAdd()
        {
            string sql = "CREATE PARTITION SCHEME " + FullName + "\r\n";
            sql += " AS PARTITION " + partitionFunction + "\r\n";
            sql += "TO (";
            fileGroups.ForEach(item => sql += "[" + item + "],");
            sql = sql.Substring(0, sql.Length -1);
            sql += ")\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP PARTITION SCHEME " + FullName + "\r\nGO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropPartitionScheme);
            }
            if (this.Status == Enums.ObjectStatusType.RebuildStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropPartitionScheme);
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddPartitionScheme);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddPartitionScheme);
            }
            return listDiff;               
        }

        public static Boolean Compare(PartitionScheme origen, PartitionScheme destino)
        {
            if (destino == null) throw new ArgumentNullException("destino");
            if (origen == null) throw new ArgumentNullException("origen");
            if (!origen.PartitionFunction.Equals(destino.PartitionFunction)) return false;
            if (origen.FileGroups.Count != destino.FileGroups.Count) return false;
            for (int j = 0; j < origen.FileGroups.Count; j++)
            {
                if (origen.CompareFullNameTo(origen.FileGroups[j], destino.FileGroups[j]) != 0)
                    return false;
            }
            return true;
        }
    }
}
