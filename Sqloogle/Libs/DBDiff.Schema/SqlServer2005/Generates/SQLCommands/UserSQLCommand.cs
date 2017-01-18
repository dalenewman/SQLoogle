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
using System.Text;
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.SQLCommands
{
    internal static class UserSQLCommand
    {
        public static string Get(DatabaseInfo.VersionTypeEnum version)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008 ||
                version == DatabaseInfo.VersionTypeEnum.SQLServer2008R2)
                return Get2008();

            //fall back to highest compatible version
            return GetAzure();

        }

        private static string Get2008()
        {
            var sql = new StringBuilder();
            sql.AppendLine("/* SQLoogle */ SELECT is_fixed_role, type, ISNULL(suser_sname(sid),'') AS Login,Name,principal_id, ISNULL(default_schema_name,'') AS default_schema_name ");
            sql.AppendLine("FROM sys.database_principals ");
            sql.AppendLine("WHERE type IN ('S','U','A','R') ");
            sql.AppendLine("ORDER BY Name");
            return sql.ToString();
        }

        private static string GetAzure()
        {
            var sql = new StringBuilder();
            //to get LoginName in Azure (asside for the current login) you would have to link to master and query sys.sysusers or sys.sql_users
            //the CASE test below will at least get you the Current login
            sql.AppendLine("/* SQLoogle */ SELECT is_fixed_role, type, CASE WHEN suser_sid()=sid THEN suser_sname() ELSE '' END  AS Login,Name,principal_id, ISNULL(default_schema_name,'') AS default_schema_name ");
            sql.AppendLine("FROM sys.database_principals ");
            sql.AppendLine("WHERE type IN ('S','U','A','R') ");
            sql.AppendLine("ORDER BY Name");
            return sql.ToString();
        }
    }
}
