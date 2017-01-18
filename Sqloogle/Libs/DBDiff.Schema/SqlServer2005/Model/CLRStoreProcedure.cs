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
using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class CLRStoreProcedure:CLRCode
    {
        private List<Parameter> parameters;

        public CLRStoreProcedure(ISchemaBase parent)
            : base(parent, Enums.ObjectType.CLRStoreProcedure, Enums.ScripActionType.AddStoreProcedure, Enums.ScripActionType.DropStoreProcedure)
        {
            parameters = new List<Parameter>();
        }

        public List<Parameter> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public override string ToSql()
        {
            string sql = "CREATE PROCEDURE " + FullName + "\r\n";
            string param = "";
            parameters.ForEach(item => param += "\t" + item.ToSql() + ",\r\n");
            if (!String.IsNullOrEmpty(param)) param = param.Substring(0, param.Length - 3) + "\r\n";
            sql += param;
            sql += "WITH EXECUTE AS " + AssemblyExecuteAs + "\r\n";
            sql += "AS\r\n";
            sql += "EXTERNAL NAME [" + AssemblyName + "].[" + AssemblyClass + "].[" + AssemblyMethod + "]\r\n";
            sql += "GO\r\n";
            return sql;
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList list = new SQLScriptList();

            if (this.HasState(Enums.ObjectStatusType.DropStatus))
                list.Add(Drop());
            if (this.HasState(Enums.ObjectStatusType.CreateStatus))
                list.Add(Create());
            if (this.Status == Enums.ObjectStatusType.AlterStatus)
            {
                list.AddRange(Rebuild());
            }
            list.AddRange(this.ExtendedProperties.ToSqlDiff());
            return list;
        }
    }
}
