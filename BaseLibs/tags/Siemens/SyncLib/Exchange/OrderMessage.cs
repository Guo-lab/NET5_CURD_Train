using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace SyncLib.Exchange
{
    public class OrderMessage:Message
    {
        public string OrderId { get; set; }
        public string OrderType { get; set; }
        public override void SaveTo(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            File.WriteAllText(folder + "\\" + OrderType + "-" + OrderId + ".json", this.MessageInfo);
        }
    }
}
