namespace ProjectBase.DomainEvent
{
    public interface IDomainEventOutProcSubscriber
    {
        public string? ConnectionName { get; set; }

        /// <summary>
        /// 启动订阅
        /// </summary>
        /// <param name="subscriberName"></param>
        /// <param name="publisherNames"></param>
        /// <param name="domainEventNS"></param>
        void Init(string subscriberName,string publisherNames, string domainEventNS);

        /// <summary>
        /// 处理失败的事件恢复处理
        /// </summary>
        void RestoreHandle();
    }
}
