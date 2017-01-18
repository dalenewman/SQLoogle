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
using Sqloogle.Libs.DBDiff.Schema.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model
{
    public class User : SQLServerSchemaBase
    {
        private string login;

        public User(ISchemaBase parent)
            : base(parent, Enums.ObjectType.User)
        {
        }

        public override string FullName
        {
            get { return "[" + Name + "]"; }
        }

        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        public override string ToSql()
        {
            string sql = "";
            sql += "CREATE USER ";
            sql += FullName + " ";
            if (!String.IsNullOrEmpty(login))
                sql += "FOR LOGIN [" + login + "] ";
            else
                sql += "WITHOUT LOGIN ";
            if (!String.IsNullOrEmpty(Owner))
                sql += "WITH DEFAULT_SCHEMA=[" + Owner + "]";
            return sql.Trim() + "\r\nGO\r\n";
        }

        public override string ToSqlDrop()
        {
            return "DROP USER " + FullName + "\r\nGO\r\n";
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
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropUser);
            }
            if (this.Status == Enums.ObjectStatusType.CreateStatus)
            {
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddUser);
            }
            if ((this.Status & Enums.ObjectStatusType.AlterStatus) == Enums.ObjectStatusType.AlterStatus)
            {
                listDiff.Add(ToSqlDrop(), 0, Enums.ScripActionType.DropUser);
                listDiff.Add(ToSql(), 0, Enums.ScripActionType.AddUser);
            }
            return listDiff;
        }

        public bool Compare(User obj)
        {
            if (obj == null) throw new ArgumentNullException("destino");
            if (!this.Login.Equals(obj.Login)) return false;
            if (!this.Owner.Equals(obj.Owner)) return false;
            return true;
        }
    }
}
