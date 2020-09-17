using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public static class DateTimeExtension
    {
        public static string ToMySqlDateString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static string ToMySqlDateTimeString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:dd");
        }

        public static string ToMySqlTimeString(this DateTime dt)
        {
            return dt.ToString("HH:mm:00");
        }
    }
}
