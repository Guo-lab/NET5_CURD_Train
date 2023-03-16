using System;

namespace E2biz.Utility.General
{
    public static class VersionUtil
    {
        public static string VersionToChar(this int? version)
        {
            if (version.HasValue)
            {
                return Convert.ToString((char)((int)'A' + version - 1));
            }
            else {
                return "";
            }
        }
    }
}
