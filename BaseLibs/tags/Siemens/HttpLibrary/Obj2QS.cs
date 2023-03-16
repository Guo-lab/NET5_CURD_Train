using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Linq;

namespace HttpLibrary
{
    public class Obj2QS
    {
        public static IDictionary<string, object> Obj2QsDict(object obj, IDictionary<string, object> map = null, String enclosingPropName = "", Type objType = null)
        {
            if (map == null) map = new Dictionary<string, object>();

            var type = objType != null ? objType : obj.GetType();
            var typedef = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (obj == null)
            {
                map.Add(enclosingPropName, "");
            }
            else if (type.IsValueType || type == typeof(System.String))
            {
                if (type == typeof(System.DateTime) || type==typeof (System.DateTime?))
                    map.Add(enclosingPropName, ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    map.Add(enclosingPropName, obj.ToString());
            }
            else if (type.IsArray)
            {
                var array = (Array)obj;
                for (var i = 0; i < array.GetLength(0); i++)
                {
                    Obj2QsDict(array.GetValue(i), map, enclosingPropName + "[" + i + "]");
                }
            }
            else if (type == typeof(IList) || type.GetInterfaces().Contains(typeof(IList)) || (typedef != null && (typedef == typeof(IList<>) || typedef.GetInterfaces().Contains(typeof(IList)))))
            {
                var list = (IList)obj;
                if (list.Count == 0)
                {
                    map.Add(enclosingPropName, "[]");
                }
                else {
                    for (var i = 0; i < list.Count; i++)
                    {
                        Obj2QsDict(list[i], map, enclosingPropName + "[" + i + "]");
                    }
                }
            }
            else if (type == typeof(IDictionary) || type.GetInterfaces().Contains(typeof(IDictionary)) || (typedef != null && (typedef == typeof(IDictionary<,>) || typedef.GetInterfaces().Contains(typeof(IDictionary)))))
            {
                var dict = (IDictionary)obj;
                foreach (var key in dict.Keys)
                {
                    Obj2QsDict(dict[key], map, enclosingPropName + "[" + key + "]");
                }
            }
            else
            {
                var props = obj.GetType().GetProperties();
                Array.ForEach(props, p =>
                {
                    var pname = (enclosingPropName == "" ? "" : (enclosingPropName + ".")) +
                                p.Name;
                    Obj2QsDict(p.GetValue(obj, null), map, pname, p.PropertyType);
                });
            }


            return map;

        }
        public static String ConvertToQS(object obj)
        {
            var map = Obj2QsDict(obj);
            if (map.Keys.Count == 0) { return ""; }
            var qs = "";
            foreach (var name in map.Keys)
            {
                qs = qs + "&" + HttpUtility.UrlEncode(name) + "=" + HttpUtility.UrlEncode (map[name].ToString());
            }
            return qs.Substring(1);
        }
        
    }
}
