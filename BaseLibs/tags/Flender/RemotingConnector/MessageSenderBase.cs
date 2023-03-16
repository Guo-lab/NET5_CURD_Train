using CoreRemoting;


namespace RemotingConnector
{
    /*
     * 所有需要通过Remoting方式发送消息的基类
     * 实现步骤 以下以发送JobOrder为例说明
     *             1 定义需要发送的消息类型JobOrder
     *             2 定义要在远程执行的对象的接口如IJobEvent, 接口中定义一个Send方法
     *             3 定义IJobEvent的实现类RemotingJob，此类继承自RemotingService<JobOrder>
     *             4 定义发送消息的接口IJobMessageSender，只有一个方法Send
     *             5 在服务器端注册RemotingServer对象并start，并为RemotingJob的静态委托SendEvent设置处理程序，这段程序是客户端远程调用后需要执行的代码
     *             6 在客户端定义IMessageSender<IJobEvent>实现类JobMessageSender，继承自MessageSenderBase, 并调用Send方法向服务器端发送数据
     */
    public class MessageSenderBase<T>
    {
        private RemotingClient _client = null;

        T _proxy;
        protected T Proxy
        {
            get
            {
                if (_proxy == null)
                {
                    _proxy = _client.CreateProxy<T>();
                }
                return _proxy;
            }
        }
        public MessageSenderBase(string server, int port)
        {
            _client = new RemotingClient(new ClientConfig()
            {
                ServerHostName = server,
                ServerPort = port
            });
        }

        protected void ConnectToServer()
        {
            if (!_client.IsConnected)
            {
                _client.Connect();
            }

        }

        ~MessageSenderBase()
        {
            if (_client!=null && _client.IsConnected)
            {
                _client.Disconnect();
                _client.Dispose();
            }
        }

    }
}
