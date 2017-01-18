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
using System.Collections.ObjectModel;

namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Options
{
    public class SqlOptionFilter
    {
        private Collection<SqlOptionFilterItem> items;

        public SqlOptionFilter()
        {
            items = new Collection<SqlOptionFilterItem>();
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Table,"dbo.dtproperties"));
            /* // TODO: Filter by name doesn't seem to be implemented?
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Assembly, "Microsoft.SqlServer.Types"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_accessadmin"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_backupoperator"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_datareader"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_datawriter"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_ddladmin"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_denydatareader"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_denydatawriter"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_owner"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "db_securityadmin"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "dbo"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "guest"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "INFORMATION_SCHEMA"));
            Items.Add(new SqlOptionFilterItem(Enums.ObjectType.Schema, "sys"));
            //*/
        }

        public Collection<SqlOptionFilterItem> Items
        {
            get { return items; }
        }

    }
}
