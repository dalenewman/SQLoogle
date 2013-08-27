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
    /// The SQLoogle MIA (Missing Index Analyzer) process.
    /// </summary>
    public class SqloogleMiaProcess : SqloogleEtlProcess {

        protected override void Initialize() {

            var config = (SqloogleBotConfiguration)ConfigurationManager.GetSection("sqloogleBot");
            var miaFolder = Path.Combine(config.SearchIndexPath, "MIA");

            if (!Directory.Exists(miaFolder))
                Directory.CreateDirectory(miaFolder);

            using (var writer = new LuceneWriter(miaFolder)) {
                writer.Clean();
            }

            Register(new ParallelUnionAllOperation(config.ServerMiaCrawlProcesses()));
            Register(new MissingIndexTransform());
            RegisterLast(new LuceneLoad(miaFolder));
        }
    }
}
