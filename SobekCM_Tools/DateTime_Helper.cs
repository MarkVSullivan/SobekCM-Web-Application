#region Using directives

using System;

#endregion

namespace SobekCM.Tools
{
    /// <summary> Static class contains helper methods for working with date/times  </summary>
    public static class DateTime_Helper
    {
        /// <summary> Returns the RFC-822 date format for a particulat date time </summary>
        /// <param name="Date"> Date to display in RFC-822</param>
        /// <returns> Date as the RFC-822 format </returns>
        public static string ToRfc822(DateTime Date)
        {
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
            string timeZone = "+" + offset.ToString().PadLeft(2, '0');

            if (offset < 0)
            {
                int i = offset*-1;
                timeZone = "-" + i.ToString().PadLeft(2, '0');
            }

            return Date.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone.PadRight(5, '0'));
        }
    }
}
