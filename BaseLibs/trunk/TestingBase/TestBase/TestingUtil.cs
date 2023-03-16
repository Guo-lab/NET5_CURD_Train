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
        public static string GetToolSetting(string key)
        {
            var path=(AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "toolsettings.json";
            dynamic obj=JsonConvert.DeserializeObject(File.ReadAllText(path));
            return obj[key];
        }
        public static string RealPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) +relativePath;
        }
        public static void WriteObjectFile(object obj, string objectName = "", string relativeFilename="")
        {
            objectName = obj.GetType().Name + "_" + objectName;
            relativeFilename = relativeFilename  + objectName + ".json";
            File.WriteAllText(GetToolSetting("ToolOutputFolder") + "\\"+ relativeFilename, JsonConvert.SerializeObject(obj));
        }
        public static T ReadObjectFile<T>(string relativePath)
        {
            var s = File.ReadAllText(RealPath(relativePath));
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
