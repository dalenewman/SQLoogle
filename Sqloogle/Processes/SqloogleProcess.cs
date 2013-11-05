/*
   Copyright 2013 Dale Newman

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Configuration;
using System.IO;
using Sqloogle.Operations;
using Sqloogle.Operations.Support;

namespace Sqloogle.Processes {

    /// <summary>
    /// The SQLoogle process.  This is where it all comes together.  It SQLoogles!
    /// </summary>
    public class SqloogleProcess : SqloogleEtlProcess {

        protected override void Initialize() {

            var config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");

            if (!Directory.Exists(config.SearchIndexPath))
                Directory.CreateDirectory(config.SearchIndexPath);

            Register(new ParallelUnionAllOperation(config.ServerCrawlProcesses()));
            Register(new SqloogleAggregate());
            Register(new SqloogleTransform());
            Register(new SqloogleCompare().Right(new LuceneExtract(config.SearchIndexPath)));
            Register(new LuceneLoad(config.SearchIndexPath));
        }
    }
}
