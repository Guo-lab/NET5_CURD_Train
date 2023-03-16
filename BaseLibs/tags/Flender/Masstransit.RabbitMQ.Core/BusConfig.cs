using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masstransit.RabbitMQ.Core
{
	/// <summary>
	/// 消息总线 传输层配置
	/// </summary>
	public class BusConfig
	{
        public string Port { get; set; }
        public string Host { get; set; }

        public string Ip { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string VirtualHost { get; set; }


        public string EndpointName { get; set; } = string.Empty;
        public int Retry { get; set; } = 0;
        public bool IsTransaction { get; set; } = false;
        public ushort PrefetchCount { get; set; } = 128;
    }
}
