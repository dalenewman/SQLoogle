﻿/*
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

using Rhino.Etl.Core.Operations;
using Sqloogle.Operations;

namespace Sqloogle.Processes {

    /// <summary>
    /// A DefinitionProcess queries object definitions from each database
    /// on a server.  It also queries statistics (use and last used).
    /// </summary>
    public class DefinitionProcess : PartialProcessOperation {

        public DefinitionProcess(string connectionString) {
            Register(new DatabaseExtract(connectionString));
            Register(new DatabaseFilter());
            Register(new DefinitionExtract());
            Register(new CachedObjectStatsJoin().Right(new CachedObjectStatsExtract(connectionString)));
            Register(new TableStatsJoin().Right(new TableStatsExtract(connectionString)));
            RegisterLast(new IndexStatsJoin().Right(new IndexStatsExtract(connectionString)));
        }
    }
}
