﻿using Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Model;

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
