using Newtonsoft.Json;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator
{
    public interface IMessageKeeper<TPublish,THandle> 
    {
        /// <summary>
        /// 保存未成功发布的消息（并且不在重发队列中），以便后续恢复发送
        /// </summary>
        /// <param name="message"></param>
        void KeepToPublish(object message,string reason);
        /// <summary>
        /// 返回所有消息留存ID和消息对象
        /// </summary>
        /// <returns></returns>
        Dictionary<string, TPublish> GetToPublish();
        /// <summary>
        /// 标记留存消息已发布成功
        /// </summary>
        /// <param name="keepId"></param>
        void MarkAsDonePublish(string keepId);
        void KeepToHandle(object message, string reason="");
        Dictionary<string, THandle> GetToHandle();
        void MarkAsDoneHandle(string keepId);
    }
   
}
