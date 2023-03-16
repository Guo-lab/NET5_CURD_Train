using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
namespace TestingBase.TestBase
{
	public class AssertUtil
	{
		private static readonly string ERR_MSG = "实际与预期不符: ";
		public static bool AssertMapDataEquals(object expected, object actual, string errmsgPrefix="")
		{
			string err = errmsgPrefix + ERR_MSG;
			if (actual == null && expected != null)
			{
				Fail(err + "expected=" + expected.ToString() + ",actual=null");
			}
			if (actual != null && expected == null)
			{
				Fail(err + "expected=null,actual=" + actual.ToString());
			}
			if ((actual == null && expected == null) || actual.Equals(expected) || expected.Equals("@@any"))
			{
				return true;
			}
            if (expected is JArray jexpected)
            {
				if(actual is JArray jactual)
                {
					var actualArray = jactual.ToArray<object>();
					var expectedArray = jexpected.ToArray();
					for (var i = 0; i < actualArray.Length; i++)
					{
						object actualValue = actualArray[i];
						object expectedValue = expectedArray[i];
						if (!AssertMapDataEquals(expectedValue, actualValue, errmsgPrefix))
						{
							Fail(err + "property=" + i
								+ ",expected=" + expectedValue + ",actual=" + actualValue);
						}
					}
				}
                else 
				{
					Fail(err + "expected=" + expected.ToString() + ",actual=" + actual.ToString());
                }
				return true;
			}

			Dictionary<string, object> actualMap = null;
			Dictionary<string, object> expectedMap = null;
			try
			{
				var s =JsonConvert.SerializeObject(actual);
				actualMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(s);
				s =JsonConvert.SerializeObject(expected);
				expectedMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(s);
			}
			catch (Exception)
			{
				Fail(err + "expected=" + expected.ToString() + ",actual=" + actual.ToString());
			}
			foreach (var entry in expectedMap)
			{
				string key = entry.Key;
				if (!actualMap.ContainsKey(key))
				{
					Fail(err + "预期属性不在实际结果中: " + key);
				}
				object actualValue = actualMap[key];
				object expectedValue = entry.Value;
				if (!AssertMapDataEquals(expectedValue, actualValue, errmsgPrefix))
				{
					Fail(err + "property=" + key
						+ ",expected=" + expectedValue + ",actual=" + actualValue);
				}
			}
			return true;
		}
		public static void AssertMapDataCollectionEquals(IEnumerable<object> expected, IEnumerable<object> actual, string errmsgPrefix = "")
		{
			var cnt = expected.Count();
			for(var i = 0; i < cnt; i++)
            {
				AssertMapDataEquals(expected.ElementAt(i), actual.ElementAt(i), errmsgPrefix);
			}
		}
		private static void Fail(string errmsg)
        {
			Assert.True(false, errmsg);
		}

		/**
		 * 断言对象的字符串值相同，大小写不敏感
		 * @param obj
		 * @param s
		 */
		public static void AssertSE(string expected,object actural)
		{
			Assert.NotNull(actural);
			Assert.Equal(expected, actural.ToString(), true);
		}
		/**
		 * 断言两个字符串相同，大小写不敏感
		 * @param s1
		 * @param s2
		 */
		public static void AssertSE(string expected,String actural)
		{
			Assert.NotNull(actural);
			Assert.Equal(expected, actural,true);
		}
	}
}
