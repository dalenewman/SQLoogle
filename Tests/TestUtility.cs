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
using NUnit.Framework;
using Sqloogle.Utilities;

namespace Tests {

    [TestFixture]
    public class TestUtility {

        [Test]
        public void TestSplitTitleCase1() {
            const string input = "ImportTitleStock";
            const string expected = "Import Title Stock";
            Assert.AreEqual(expected, Strings.SplitTitleCase(input, " "));
        }

        [Test]
        public void TestSplitTitleCase2() {
            const string input = "Import_Title_Stock";
            const string expected = "Import Title Stock";
            Assert.AreEqual(expected, Strings.SplitTitleCase(input, " "));
        }

        [Test]
        public void TestRemoveSqlPunctuation1() {
            const string input = "select * from [database].[schema].[table];";
            const string expected = "select * from  database   schema   table  ";
            Assert.AreEqual(expected, SqlStrings.RemoveSqlPunctuation(input));
        }

        [Test]
        public void TestUseBucket()
        {
            Assert.AreEqual("0000000000", Strings.UseBucket(0));
            Assert.AreEqual("0000000005", Strings.UseBucket(5));
            Assert.AreEqual("0000000010", Strings.UseBucket(10));
            Assert.AreEqual("0000000070", Strings.UseBucket(78));
            Assert.AreEqual("0000000100", Strings.UseBucket(152));
            Assert.AreEqual("0000000300", Strings.UseBucket(367));
            Assert.AreEqual("0000001000", Strings.UseBucket(1027));
            Assert.AreEqual("0000003000", Strings.UseBucket(3875));
            Assert.AreEqual("0003000000", Strings.UseBucket(3820083));

            Assert.AreEqual("0000000000", Strings.UseBucket(null));
            Assert.AreEqual("0000000000", Strings.UseBucket(""));

        }

    }
}