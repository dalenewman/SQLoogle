﻿using Sqloogle.Libs.DBDiff.Schema.Model;

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
