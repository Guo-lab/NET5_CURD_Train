using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Domain;
using System.Linq.Expressions;
using SyncLib.Exchange;
using System.Reflection;
using SyncLib.Exception;
using System.Collections;
using Newtonsoft.Json;
using AutoMapper;

namespace SyncLib
{
    public abstract class SyncBase<T,Tid,TDTO>: ISynchronizeHandler where T : BaseDomainObjectWithTypedId<Tid>, ICanSynchronize
    {
        public IMapper Mapper { get; set; }
        protected AfterDownloadHandler AfterDownload;
        protected void SetHandler(AfterDownloadHandler handler)
        {
            this.AfterDownload = handler;
        }
        public SyncBase() { }

        protected abstract Expression<Func<T, bool>> GetFilter();
        public IList<Message> GetDownloadableOrder()
        {
            IList<T> arrT = dao.GetByQuery(GetFilter());
            List<Message> arrMsg = new List<Message>();
            foreach (T order in arrT)
            {
                IList<Message> orderMessage = GetOrderMessage(order);
                arrMsg.AddRange(orderMessage);
                if (orderMessage.Count(x => x is InfoMessage) == 0)
                {
                    order.SyncTime = DateTime.Now;
                    if (AfterDownload != null)
                        AfterDownload(order);
                }
            }
            return arrMsg;
        }

        public IGenericDaoWithTypedId<T, Tid> dao { get; set; }
        public virtual string OrderType
        {
            get; set;
        }
        public virtual void ManualCreateProperty(T order, TDTO orderDTO)
        {

        }
        protected IList<Message> GetOrderMessage(T order)
        {

            List<Message> arrMsg = new List<Message>();

            TDTO orderDTO = Mapper.Map<T, TDTO>(order);
            ManualCreateProperty(order, orderDTO);
            IList<Message> attachmentMsg = GetAttachmentProperty(orderDTO!);
            if (attachmentMsg.Count > 0)
            {
                arrMsg.AddRange(attachmentMsg);
                if (attachmentMsg.Count(x => x is InfoMessage) > 0)
                    return arrMsg;
            }
            arrMsg.Add(new OrderMessage
            {
                OrderId = order.Id!.ToString()!,
                OrderType = this.OrderType,
                MessageInfo = JsonConvert.SerializeObject(orderDTO)
            });

            return arrMsg;
        }

        private IList<Message> GetAttachmentProperty(object order)
        {
            List<Message> message = new List<Message>();
            var t = order.GetType();

            if (typeof(Attachment).IsAssignableFrom(t))
            {
                Message? msg = GetAttachmentMessage(null, order);
                if (msg != null)
                {
                    message.Add(msg);
                }
                return message;
            }

            t.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(x =>
            {
                if (typeof(Attachment).IsAssignableFrom(x.PropertyType))
                {
                    Message? msg = GetAttachmentMessage(x, order);
                    if (msg != null)
                    {
                        message.Add(msg);
                    }
                }
                else {
                    var tp = x.PropertyType;
                    var typedef = tp.IsGenericType ? tp.GetGenericTypeDefinition() : null;
                    if (tp == typeof(IList) || tp.GetInterfaces().Contains(typeof(IList)) ||
                     (typedef != null && (typedef == typeof(IList<>) || typedef.GetInterfaces().Contains(typeof(IList)))))
                    {
                        var list = (IList?)x.GetValue(order, null);
                        if (list != null)
                        {
                            for (var i = 0; i < list.Count; i++)
                            {
                                message.AddRange(GetAttachmentProperty(list[i]!));
                            }
                        }
                    }
                }
            });
            return message;
        }

        private Message? GetAttachmentMessage(PropertyInfo? pi, object order)
        {
            try
            {
                Attachment? attachment;
                if (pi == null)
                    attachment = (Attachment)order;
                else
                    attachment = (Attachment?)pi.GetValue(order, null);
                if (attachment == null)
                {
                    return null;
                }
                else {
                    return new AttachmentMessage
                    {
                        TargetFolder = attachment.TargetFolder,
                        SavedFileName = attachment.SavedFileName,
                        MessageInfo = attachment.GetAttachmentContent()
                    };
                }
            }
            catch (FileMissingException e)
            {
                return new InfoMessage { MessageInfo = e.Message };
            }
            catch (ReadFileException e)
            {
                return new InfoMessage { MessageInfo = e.Message };
            }
        }

    }
}
