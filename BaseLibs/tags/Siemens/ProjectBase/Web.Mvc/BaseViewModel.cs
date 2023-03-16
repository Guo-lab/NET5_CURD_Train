using Newtonsoft.Json;

namespace ProjectBase.Web.Mvc
{
    public interface BaseViewModel
    {
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public string ToJsonString(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }

  

}
