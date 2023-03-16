using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilService
{
    public static class DateUtil
    {
        public static string GetWeekDay(this DateTime date)
        {
            string[] weekDays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            return weekDays[Convert.ToInt32(date.DayOfWeek)];
        }

        public static string Duration(this int minutes)
        {
            int hour = minutes / 60;
            return hour.ToString("00") + ":" + (minutes % 60).ToString("00");
        }
    }
}
