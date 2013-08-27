using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Sqloogle.Libs.Rhino.Etl.Core;
using Sqloogle.Libs.Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations {
    public class DatabaseFilter : AbstractOperation {

        private readonly SqloogleBotConfiguration _config;

        public DatabaseFilter(){
            _config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return rows.Where(row => !_config.Skips.Match(row["database"].ToString()));
        }
    }
}
