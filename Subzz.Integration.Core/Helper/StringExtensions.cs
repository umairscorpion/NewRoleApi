using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace Subzz.Integration.Core.Helper
{
    public static class StringExtensions
    {
        public static string BlindReplace(this string str, string oldStr, string newStr)
        {
            return Regex.Replace(str, oldStr, newStr, RegexOptions.IgnoreCase);
        }
        public static string ToSubzzTime(this DateTime dateTime)
        {
            return dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
        }

        public static string ToSubzzTimeForSms(this DateTime dateTime)
        {
            return dateTime.ToString("hh:mmt", CultureInfo.InvariantCulture);
        }

        public static string ToSubzzDateForSMS(this DateTime dateTime)
        {

            return dateTime.ToString("MM/dd/yy(ddd)");
        }
    }
}
