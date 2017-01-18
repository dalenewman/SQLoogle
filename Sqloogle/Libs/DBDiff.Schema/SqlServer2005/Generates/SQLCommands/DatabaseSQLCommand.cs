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
using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Generates.SQLCommands
{
    internal class DatabaseSQLCommand
    {
        public static string GetVersion(Database databaseSchema)
        {
            const string sql = "/* SQLoogle */ SELECT SERVERPROPERTY('productversion') AS Version, SERVERPROPERTY('EngineEdition') AS Edition";
            return sql;
        }

        public static string Get(DatabaseInfo.VersionTypeEnum version, Database databaseSchema)
        {
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2005) return Get2005(databaseSchema);
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2008) return Get2008(databaseSchema);
            if (version == DatabaseInfo.VersionTypeEnum.SQLServer2008R2) return Get2008R2(databaseSchema);
            if (version == DatabaseInfo.VersionTypeEnum.SQLServerAzure10) return GetAzure(databaseSchema);
            return "";
        }

        private static string Get2005(Database databaseSchema)
        {
            string sql = "/* SQLoogle */ SELECT DATABASEPROPERTYEX('" + databaseSchema.Name + "','IsFulltextEnabled') AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }

        private static string Get2008(Database databaseSchema)
        {
            string sql = "/* SQLoogle */ SELECT DATABASEPROPERTYEX('" + databaseSchema.Name + "','IsFulltextEnabled') AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }

        private static string Get2008R2(Database databaseSchema)
        {
            string sql = "/* SQLoogle */ SELECT DATABASEPROPERTYEX('" + databaseSchema.Name + "','IsFulltextEnabled') AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }

        private static string GetAzure(Database databaseSchema)
        {
            //DATABASEPROPERTYEX('IsFullTextEnabled') is deprecated http://technet.microsoft.com/en-us/library/cc646010(SQL.110).aspx
            string sql = "/* SQLoogle */ SELECT 0 AS IsFullTextEnabled, DATABASEPROPERTYEX('" + databaseSchema.Name + "','Collation') AS Collation";
            return sql;
        }
    }
}
