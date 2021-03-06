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
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Etl.Core;
using Sqloogle.Operations;
using Sqloogle.Operations.Support;

namespace Tests {
    [TestFixture]
    public class TestSqloogleCompare : EtlProcessHelper {

        [Test]
        public void Test1() {

            //"use", "lastused", "modified", "count", "created", "name", "server", "database", "schema", "dropped"
            var fresh = new FakeOperation(
                new List<Row> {
                    new Row {{"id", 1}, {"use", "old1"}, {"lastused", ""}, {"modified", "old2"}, {"count", ""}, {"created",""}, {"name",""}, {"server",""}, {"database",""}, {"schema",""},{"dropped",false}},
                    new Row {{"id", 2}, {"use", "old1"}, {"lastused", ""}, {"modified", "same"}, {"count", "3"}, {"created",""}, {"name",""}, {"server",""}, {"database",""}, {"schema",""},{"dropped",false}},
                    new Row {{"id", 3}, {"use", "new1"}, {"lastused", ""}, {"modified", "new2"}, {"count", ""}, {"created",""}, {"name",""}, {"server",""}, {"database",""}, {"schema",""},{"dropped",false}}
                }
            );

            var existing = new FakeOperation(
                new List<Row> {
                    new Row {{"id", 1}, {"use", "old1"}, {"lastused", ""}, {"modified", "old2"}, {"count", ""}, {"created",""}, {"name",""}, {"server",""}, {"database",""}, {"schema",""},{"dropped",false}},
                    new Row {{"id", 2}, {"use", "old1"}, {"lastused", ""}, {"modified", "same"}, {"count", "2"}, {"created",""}, {"name",""}, {"server",""}, {"database",""}, {"schema",""},{"dropped",false}},
                    new Row {{"id", 4}, {"use", "old1"}, {"lastused", ""}, {"modified", "old2"}, {"count", ""}, {"created",""}, {"name",""}, {"server",""}, {"database",""}, {"schema",""},{"dropped",false}}
                }
            );
            
            var results = TestOperation(
                new SqloogleCompare().Left(fresh).Right(existing)
                ,new LogOperation()
            );

            Assert.AreEqual(4, results.Count, "It's a full outer join, we should have all 4 rows!");
            Assert.AreEqual("None", results[0]["Action"], "These rows are the same; should be None!");
            Assert.AreEqual("Update", results[1]["Action"], "There is an updated property; should be Update!");
            Assert.AreEqual("Create", results[2]["Action"], "This has never been seen before; should be Create!");
            Assert.AreEqual("Update", results[3]["Action"], "This is gone! should be an Update!");
            Assert.AreEqual(true, results[3]["Dropped"], "We don't delete drops, we mark them as dropped!");

        }
    }
}

