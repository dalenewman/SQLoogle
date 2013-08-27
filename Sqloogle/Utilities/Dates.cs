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