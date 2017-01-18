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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {
    public class DatabaseFilter : AbstractOperation {

        private readonly SqloogleBotConfiguration _config;

        public DatabaseFilter()
        {
            UseTransaction = false;
            _config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return rows.Where(row => !_config.Skips.Match(row["database"].ToString()));
        }
    }
}
