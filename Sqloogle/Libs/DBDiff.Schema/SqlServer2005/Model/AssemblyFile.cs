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
using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class AssemblyFile : SQLServerSchemaBase
    {
        private string content;

        public AssemblyFile(ISchemaBase parent, AssemblyFile assemblyFile, Enums.ObjectStatusType status)
            : base(parent, Enums.ObjectType.AssemblyFile)
        {
            this.Name = assemblyFile.Name;
            this.content = assemblyFile.content;
            this.Status = status;
        }

        public AssemblyFile(ISchemaBase parent, string name, string content)
            : base(parent,Enums.ObjectType.AssemblyFile)
        {
            this.Name = name;
            this.content = content;
        }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public override string ToSqlAdd()
        {
            string sql = "ALTER ASSEMBLY ";
            sql += this.Parent.FullName + "\r\n";
            sql += "ADD FILE FROM " + this.Content + "\r\n";
            sql += "AS N'" + this.Name + "'\r\n";
            return sql + "GO\r\n";
        }

        public override string ToSql()
        {
            return ToSqlAdd();
        }

        public override string ToSqlDrop()
        {
            string sql = "ALTER ASSEMBLY ";
            sql += this.Parent.FullName + "\r\n";
            sql += "DROP FILE N'" + this.Name + "'\r\n";
            return sql + "GO\r\n";
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropAssemblyFile);
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddAssemblyFile);
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropAssemblyFile);
                listDiff.Add(ToSqlAdd(), 0, Enums.ScripActionType.AddAssemblyFile);
            }
            return listDiff;
        }
    }
}
