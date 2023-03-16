using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingBase.TestBase
{
    public class TestingUtil
    {
        public static string GetAppSetting(string key)
        {
            var path=(AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\appSettings.json";
            dynamic obj=JsonConvert.DeserializeObject(File.ReadAllText(path));
            return obj.AppSetting[key];
        }

    }
}
