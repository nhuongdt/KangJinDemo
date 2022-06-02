using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.App_API
{
    public class CvtDatetimetoTimestamps
    {
        public static long Convert(DateTime datetime)
        {
            long timestamp = ((datetime.Ticks - 621355968000000000) / 10000000) * 1000;
            return timestamp;
        }

        public static string ConvertToMiliSeconds(DateTime current)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan span = current.AddHours(-7) - dt1970;
            return span.TotalMilliseconds.ToString();
        }

        public static DateTime TimestampToDatetime(double val)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(val/1000d));
        }
    }
}