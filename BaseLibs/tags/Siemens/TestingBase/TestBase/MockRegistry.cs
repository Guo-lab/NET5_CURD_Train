using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingBase.TestBase
{
    /// <summary>
    /// 用于在BD单元测中，注册替身类作为依赖项
    /// </summary>
    public class MockRegistry
    {
        private static Dictionary<Type, Func<object>> dependencyToMockList { get; } = new Dictionary<Type, Func<object>>();

        public static bool NeedMock(Type clazz)
        {
            return dependencyToMockList.ContainsKey(clazz);
        }
        public static object GetMockObj(Type clazz)
        {
            return dependencyToMockList[clazz].Invoke();
        }
        public static void RegisterMockDependency<TService>(Func<object> factoryMethod)
        {
            var clazz = typeof(TService);
            if (dependencyToMockList.ContainsKey(clazz))
            {
                dependencyToMockList[clazz] = factoryMethod;
            }
            else
            {
                dependencyToMockList.Add(clazz, factoryMethod);
            }
        }
    }
}
