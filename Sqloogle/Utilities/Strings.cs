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
using System.Linq;
using System.Text.RegularExpressions;

namespace Sqloogle.Utilities {

    public static class Strings {

        private const string SPLIT_TITLE_CASE_PATTERN = "(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])";
        private const string BRACKETS_AND_COMMAS_PATTERN = @"[\[\],]";
        private const string BRACKETS_PATTERN = @"[\[\]]";
        private const string USE_DEFAULT = "0000000000";

        private static readonly Regex TitleCaseRegex = new Regex(SPLIT_TITLE_CASE_PATTERN, RegexOptions.Compiled);
        private static readonly Regex BracketsAndCommasRegex = new Regex(BRACKETS_AND_COMMAS_PATTERN, RegexOptions.Compiled);
        private static readonly Regex BracketsRegex = new Regex(BRACKETS_PATTERN, RegexOptions.Compiled);

        public static string SingleQuotedList(string list, char delimiter) {
            return !string.IsNullOrEmpty(list) ? SingleQuotedList(new List<string>(list.Split(delimiter))) : string.Empty;
        }

        public static string SingleQuotedList(IEnumerable<string> items) {
            return string.Join(",", items.Select(db => $"'{db}'"));
        }

        public static string SplitTitleCase(object titleCased, string delimiter) {
            return SplitTitleCase(titleCased.ToString(), delimiter);
        }

        public static string SplitTitleCase(string titleCased, string delimiter) {
            return string.Join(delimiter, SplitTitleCase(titleCased));
        }

        public static IEnumerable<string> SplitTitleCase(string titleCased) {
            return TitleCaseRegex.Split(titleCased).Select(s => s.Trim('_'));
        }

        public static string RemoveBracketsAndCommas(string input) {
            return BracketsAndCommasRegex.Replace(input, string.Empty);
        }

        public static string RemoveBrackets(string input) {
            return BracketsRegex.Replace(input, string.Empty);
        }

        public static string UseBucket(object number, int length = 10, char padChar = '0') {
            if (number == null)
                return USE_DEFAULT;
            var useString = number.ToString();
            return useString.Equals(string.Empty) ? USE_DEFAULT : (useString[0] + new string(padChar, useString.Length - 1)).PadLeft(length, padChar);
        }

    }
}