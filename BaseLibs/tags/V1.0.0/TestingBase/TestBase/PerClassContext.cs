using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingBase.TestBase
{
    /// <summary>
    /// 每个测试类的共享的上下文
    /// --internal  Rainy
    /// </summary>
    public class PerClassContext : IDisposable
    {
        public bool DoneSetup { get; set; }
        public Action DisposeAction { get; set; }

        public void Dispose()
        {
            DisposeAction?.Invoke();
        }
    }
}
