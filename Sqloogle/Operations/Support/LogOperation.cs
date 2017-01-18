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
using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Sqloogle.Operations.Support {
    public class LogOperation : AbstractOperation {

        private readonly string _delimiter;
        private readonly int _maxLengh;
        private readonly List<string> _ignores = new List<string>();
        private KeyValuePair<string, object> _whereFilter;

        public LogOperation(string delimiter = " | ", int maxLength = 64)
        {
            UseTransaction = false;
            _delimiter = delimiter;
            _maxLengh = maxLength;
            _whereFilter = new KeyValuePair<string, object>(string.Empty, null);
        }

        public LogOperation Ignore(string column) {
            _ignores.Add(column);
            return this;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (!String.IsNullOrEmpty(_whereFilter.Key)) {
                    if (row[_whereFilter.Key] == _whereFilter.Value) {
                        LogRow(row);
                    }
                } else {
                    LogRow(row);
                }

                yield return row;
            }
        }

        private void LogRow(Row row) {
            var values = row.Columns.Where(column => !_ignores.Contains(column)).Select(column => EnforceMaxLength(row[column])).ToList();
                Info(string.Join(_delimiter, values));
        }

        public string EnforceMaxLength(object value) {
            if (value == null)
                return string.Empty;

            var stringValue = value.ToString().Replace(Environment.NewLine, " ");

            if (stringValue.Length > _maxLengh)
                return stringValue.Substring(0, _maxLengh - 3) + "...";

            return stringValue;
        }

        public IOperation Where(string key, object value) {
            _whereFilter = new KeyValuePair<string, object>(key, value);
            return this;
        }
    }
}
