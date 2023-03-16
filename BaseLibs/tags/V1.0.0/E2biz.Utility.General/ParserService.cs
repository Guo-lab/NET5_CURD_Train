using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2biz.Utility.General
{
    public static  class ParserService
    {
        /// <summary>
        /// 数字转换，转换失败时返回 decimal.MinValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return decimal.MinValue;
            }
            else
            {
                decimal d;
                if (decimal.TryParse(value, out d))
                {
                    return d;
                }
                else
                {
                    return null;
                }
            }
        }

        public static DateTime? ParseDate(string date)
        {
            DateTime d;
            if (DateTime.TryParse(date, out d))
            {
                return d;
            }
            else
            {
                return null;
            }
        }

        public static int? ParseInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                int d;
                if (int.TryParse(value, out d))
                {
                    return d;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
