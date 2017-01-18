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

namespace Sqloogle.Utilities
{
    public static class Dates
    {

        public static DateTime ConvertDocDate(string docDate)
        {
            var year = docDate.Substring(0, 4);
            var month = docDate.Substring(4, 2);
            var day = docDate.Substring(6, 2);
            return Convert.ToDateTime(string.Concat(year, "-", month, "-", day));
        }

        public static string FormatDate(DateTime dateTime)
        {
            if (dateTime.Equals(DateTime.MinValue))
                return string.Empty;
            if (DateTime.Now.Subtract(dateTime).TotalDays > 365)
                return dateTime.ToString("yyyy");
            return DateTime.Now.Year == dateTime.Year ? dateTime.ToString("MM-dd") : dateTime.ToString("yyyy-MM-dd");
        }

    }
}