using System;
using ProjectBase.Domain;
using Newtonsoft.Json;
namespace SyncLib
{
    public abstract class ReceiveBase<T, Tid, TDTO> : IReceiveHandler where T : BaseDomainObjectWithTypedId<Tid>, IReceivable
    {
        private AfterReceiveHandler AfterReceive;

        protected void SetHandler(AfterReceiveHandler handler)
        {
            this.AfterReceive = handler;
        }
        public ReceiveBase() { }
        protected abstract T ConvertDTO2Domain(TDTO dto);

        public void ReceiveOrder(string orderInfo)
        {
            TDTO? orderDTO = JsonConvert.DeserializeObject<TDTO>(orderInfo);
            if (orderDTO == null) { return; }
            T order = ConvertDTO2Domain(orderDTO);
            order.ReceivedTime = DateTime.Now;

            if (AfterReceive != null)
            {
                AfterReceive(order);
            }

            Save(order);
        }

        protected virtual void Save(T order)
        {
            if (order.IsTransient())
            {
                dao.Save(order);
            }
            else
            {
                dao.Update(order);
            }
        }

        public IGenericDaoWithTypedId<T, Tid> dao { get; set; }
    }

}
