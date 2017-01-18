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
using System.Text.RegularExpressions;

namespace Sqloogle.Utilities
{
    public static class SqlStrings
    {
        private const string SQL_PUNCTUATION_PATTERN = @"[^\w^*]";
        private const string SQL_COMMENTS_PATTERN = @"(/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/)|(--.*$)";
        private const string SQL_OPTIONAL_PATTERN = @"((?<=\s)dbo\.|;(?=\r\n)|;$)";
        private const string SQL_OPTIONAL_NAME_PATTERN = @"[\[""]{1}\w+[^\s]\w+[\]""]{1}";
        private const string SQL_EMPTY_SINGLE_QUOTED_STRING_PATTERN = @"(?<=[^'])'{2}(?=[^'])";
        private const string SQL_COLUMN_OPERATOR_VALUE_PATTERN = @"(?<operator>[=<>!]{1,2}|like)\s*(?<value>(\-?[\d\.]+|N?'[^']*'|0x[0-9a-fA-F]+))((?=\W)|\z)";
        private const string SQL_PARAMETER_IN_FUNCTION_PATTERN = @"(?<=\(.*)(?<value>\b\-?([\d\.]{2,}|[\d]+)\b|N?'[^']*'|0x[0-9a-fA-F]+)(?=.*\))";
        private const string SQL_TABLE_PATTERN = @"(?<=(?<keywords>update|into|from|join|insert)\s+)(?<table>[\[""][\w\s\.\[\]""]+[\]""](?=([^\.]|$))|[\w\.\[\]""]+)";
        private const string SQL_EXTRAS_PATTERN = @"[\[""]{1}\w+[^\s]\w+[\]""]{1}";

        private const RegexOptions COMMON_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;

        private static readonly Regex SqlPunctuationRegex = new Regex(SQL_PUNCTUATION_PATTERN, RegexOptions.Compiled);
        private static readonly Regex SqlCommentsRegex = new Regex(SQL_COMMENTS_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlOptionalRegex = new Regex(SQL_OPTIONAL_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlNameOptionalRegex = new Regex(SQL_OPTIONAL_NAME_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlEmptySingleQuotedRegex = new Regex(SQL_EMPTY_SINGLE_QUOTED_STRING_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlColumnOperatorValueRegex = new Regex(SQL_COLUMN_OPERATOR_VALUE_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlParameterInFunctionRegex = new Regex(SQL_PARAMETER_IN_FUNCTION_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlTableRegex = new Regex(SQL_TABLE_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlExtrasRegex = new Regex(SQL_EXTRAS_PATTERN, COMMON_OPTIONS);

        
        public static string RemoveSqlPunctuation(string sql) {
            return SqlPunctuationRegex.Replace(sql, " ");
        }

        public static string RemoveSqlPunctuation(object sql) {
            return RemoveSqlPunctuation(sql.ToString());
        }
        public static string RemoveSqlComments(string sql)
        {
            return SqlCommentsRegex.Replace(sql, String.Empty);
        }

        public static string RemoveSqlOptionals(string sql)
        {
            return SqlOptionalRegex.Replace(sql, String.Empty);
        }

        public static MatchCollection MatchSqlNameOptionals(string sql)
        {
            return SqlNameOptionalRegex.Matches(sql);
        }

        public static MatchCollection MatchSqlTables(string sql)
        {
            return SqlTableRegex.Matches(sql);
        }

        public static MatchCollection MatchSqlExtras(string sql)
        {
            return SqlExtrasRegex.Matches(sql);
        }

        public static string ReplaceSqlEmptySingleQuotes(string sql, string replacement)
        {
            return SqlEmptySingleQuotedRegex.Replace(sql, replacement);
        }

        public static string ReplaceSqlColumnOperatorValues(string sql, string replacement)
        {
            return SqlColumnOperatorValueRegex.Replace(sql, replacement);
        }

        public static string ReplaceSqlParametersInFunctions(string sql, string replacement)
        {
            return SqlParameterInFunctionRegex.Replace(sql, replacement);
        }

    }
}