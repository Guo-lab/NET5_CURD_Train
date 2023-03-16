using Newtonsoft.Json;
using System;

namespace ESC5.OfflineService
{
    [Serializable]
    public class TaskBase<IdT>
    {
        public IdT Id { get; set; }

        [JsonIgnore]
        public virtual string JsonString
        {
            get
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.Objects
                                });
            }
        }
        [JsonIgnore]
        public virtual string Key
        {
            get
            {
                return this.GetType().Name + "-" + this.Id.ToString() + ".task";
            }
        }

        [JsonIgnore]
        public virtual string TypeName
        {
            get
            {
                return this.GetType().Name;
            }
        }
    }
}
