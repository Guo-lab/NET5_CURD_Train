using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketBase
{
    public interface IActionResult
    {
        object? ExecuteResult();
        ValueTask<object?> ExecuteResultAsync();
    }
    public class ActionResult : IActionResult
    {
        public ValueTask Task { get; set; }

        public virtual object? ExecuteResult()
        {
            return null;
        }
        public virtual ValueTask<object?> ExecuteResultAsync()
        {
            return ValueTask.FromResult(ExecuteResult());
        }
    }
  
}
