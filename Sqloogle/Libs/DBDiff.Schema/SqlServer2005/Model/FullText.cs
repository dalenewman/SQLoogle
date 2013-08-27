﻿using System;
using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class FullText : SQLServerSchemaBase
    {
        private string path;
        private Boolean isDefault;
        private Boolean isAccentSensity;
        private string fileGroupName;

        public FullText(ISchemaBase parent)
            : base(parent, Enums.ObjectType.FullText)
        {
            
        }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        public Boolean IsDefault
        {
            get { return isDefault; }
            set { isDefault = value; }
        }
        public Boolean IsAccentSensity
        {
            get { return isAccentSensity; }
            set { isAccentSensity = value; }
        }
        public string FileGroupName
        {
            get { return fileGroupName; }
            set { fileGroupName = value; }
        }

        public override string ToSql()
        {
            Database database = (Database)this.Parent;

            string sql = "CREATE FULLTEXT CATALOG " + FullName + " ";
            if (!IsAccentSensity)
                sql += "WITH ACCENT_SENSITIVITY = OFF\r\n";
            else
                sql += "WITH ACCENT_SENSITIVITY = ON\r\n";
            if (!String.IsNullOrEmpty(this.Path))
            {
                if (!database.Options.Ignore.FilterFullTextPath)
                    sql += "--";
                sql += "IN PATH N'" + Path + "'\r\n";
            }
            if (IsDefault)
                sql += "AS DEFAULT\r\n";
            sql += "AUTHORIZATION [" + Owner + "]\r\n";
            return sql + "GO\r\n";
        }

        private string ToSqlAlterDefault()
        {
            if (IsDefault)
            {
                string sql = "ALTER FULLTEXT CATALOG " + FullName + "\r\n";
                sql += "AS DEFAULT";
                sql += "\r\nGO\r\n";
                return sql;
            }
            else return "";

        }

        private string ToSqlAlterOwner()
        {
            string sql = "ALTER AUTHORIZATION ON FULLTEXT CATALOG::" + FullName + "\r\n";
            sql += "TO [" + Owner + "]\r\nGO\r\n";
            return sql;
        }

        private string ToSqlAlter()
        {
            string sql = "ALTER FULLTEXT CATALOG " + FullName + "\r\n";
            sql += "REBUILD WITH ACCENT_SENSITIVITY = ";
            if (IsAccentSensity) sql += "ON"; else sql += "OFF";
            sql += "\r\nGO\r\n";
            return sql;
        }

        public override string ToSqlDrop()
        {
            return "DROP FULLTEXT CATALOG " + FullName + "\r\nGO\r\n";
        }

        public override string ToSqlAdd()
        {
            return ToSql();
        }

        public override SQLScriptList ToSqlDiff()
        {
            SQLScriptList listDiff = new SQLScriptList();

            if (this.Status == Enums.ObjectStatusType.DropStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropFullText);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddFullText);
            }
            if (this.HasState(Enums.ObjectStatusType.AlterStatus))
            {
                listDiff.Add(ToSqlAlter(), 0, Enums.ScripActionType.AddFullText);
            }
            if (this.HasState(Enums.ObjectStatusType.DisabledStatus))
            {
                listDiff.Add(ToSqlAlterDefault(), 0, Enums.ScripActionType.AddFullText);
            }
            if (this.HasState(Enums.ObjectStatusType.ChangeOwner))
            {
                listDiff.Add(ToSqlAlterOwner(), 0, Enums.ScripActionType.AddFullText);
            }
            return listDiff;
        }

        /// <summary>
        /// Compara dos Synonyms y devuelve true si son iguales, caso contrario, devuelve false.
        /// </summary>
        public Boolean Compare(FullText destino)
        {
            Database database = (Database)this.Parent;
            if (destino == null) throw new ArgumentNullException("destino");
            if (!this.IsAccentSensity.Equals(destino.IsAccentSensity)) return false;
            if (!this.IsDefault.Equals(destino.IsDefault)) return false;
            if ((!String.IsNullOrEmpty(this.FileGroupName)) && (!String.IsNullOrEmpty(destino.FileGroupName)))
                if (!this.FileGroupName.Equals(destino.FileGroupName)) return false;
            if (database.Options.Ignore.FilterFullTextPath)
                if ((!String.IsNullOrEmpty(this.Path)) && (!String.IsNullOrEmpty(destino.Path)))
                    return this.Path.Equals(destino.Path, StringComparison.CurrentCultureIgnoreCase);
            return true;
        }
    }
}
