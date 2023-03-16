namespace ProjectBase.MessageSender
{
	//将消息发送到Web之外的进程（与Web可以在同一台机器也可以不在同一台机器）
    public interface IMessageSender<T> where T:class
	{
		void Send(T entity);
	}
}
