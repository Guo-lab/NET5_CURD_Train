using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLib.Exchange
{
   
    public abstract class Message
    {
        public abstract void SaveTo(string folder);
               
        public string MessageInfo { get; set; }
    }
}
