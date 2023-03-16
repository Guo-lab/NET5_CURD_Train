using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ProjectBase.Application {
    /// <summary>
    /// @author Rainy
    /// </summary>
    [Obsolete("暂无任何用途")]
    public static class CallContext {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();

        public static void SetData(string name, object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        public static object GetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;

    }
}
