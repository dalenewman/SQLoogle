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
namespace Sqloogle.Libs.DBDiff.Schema.SqlServer2005.Options
{
    public class SqlOptionFilterItem
    {
        private Enums.ObjectType type;
        private string filter;

        public SqlOptionFilterItem(Enums.ObjectType type, string value)
        {
            this.filter = value;
            this.type = type;
        }

        public Enums.ObjectType Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Filter
        {
            get { return filter; }
            set { this.filter = value; }
        }
        
    }
}
