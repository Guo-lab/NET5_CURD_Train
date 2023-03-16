using Newtonsoft.Json;
using ProjectBase.DomainEvent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMediator.DomainEvent
{
    public class DomainEventMessageKeeper : IMessageKeeper<IDomainEvent,string>
    {
        public IAlarmReporter AlarmReporter { get; set; }

        private DirectoryInfo publishDir;
        private DirectoryInfo handleDir;

        public DomainEventMessageKeeper()
        {
            publishDir = Directory.CreateDirectory(GetRunningPath("DomainEvent_KeepToPublish"));
            handleDir = Directory.CreateDirectory(GetRunningPath("DomainEvent_KeepToHandle"));
        }
        public void KeepToPublish(object message,string reason)
        {
            SaveFile(message, reason, true);
        }
        public Dictionary<string, IDomainEvent> GetToPublish()
        {
            return publishDir.GetFiles()
                .ToDictionary(o => o.Name, o => {
                    string content = File.ReadAllText(o.FullName);
                    var saved = JsonConvert.DeserializeObject<SavedDomainEvent>(content)!;
                    return (IDomainEvent)JsonConvert.DeserializeObject(saved.DomainEventObj,Type.GetType(saved.TypeName)!)!;
            });
        }
        public void MarkAsDonePublish(string keepId)
        {
            File.Delete(publishDir+"\\"+keepId);
        }
        public void MarkAsDoneHandle(string keepId)
        {
            File.Delete(handleDir + "\\" + keepId);
        }
        public void KeepToHandle(object message, string reason="")
        {
            SaveFile(message,"",false);
        }
        public Dictionary<string, string> GetToHandle()
        {
            return handleDir.GetFiles()
                .ToDictionary(o => o.Name, o => File.ReadAllText(o.FullName)!);
        }

        private void SaveFile(object message, string reason, bool publish)
        {
            string? content = message is string ? (message as string) : JsonConvert.SerializeObject(message);
            if(message is IDomainEvent)
            {
                content=JsonConvert.SerializeObject(new SavedDomainEvent()
                {
                    TypeName = message.GetType().AssemblyQualifiedName!,
                    DomainEventObj = content!
                });
            }
            var filename = GetFileName(message);
            File.WriteAllText((publish? publishDir:handleDir).FullName + "\\" + reason+"_"+ filename, content);
        }
        private string GetFileName(object message)
        {
            //IDomainEvent devent;
            //if (message is string msg)
            //{
            //    devent = JsonConvert.DeserializeObject<IDomainEvent>(msg)!;
            //}
            //else if (message is IDomainEvent)
            //{
            //    devent = (message as IDomainEvent)!;
            //}
            //else
            //{
            //    return "不支持"+new Guid().ToString();
            //}
            return Guid.NewGuid().ToString()+".json";
            
        }
        private string GetRunningPath(string relativePath)
        {
            return (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory) + "\\" + relativePath;
        }

        private class SavedDomainEvent
        {
            public string TypeName { get; set; }
            public string DomainEventObj { get; set; }
        }
    }
}
