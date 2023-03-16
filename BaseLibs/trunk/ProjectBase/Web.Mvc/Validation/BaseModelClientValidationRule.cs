using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ProjectBase.Web.Mvc.Validation
{

    /**
     * 客户端验证规则基类，支持验证分组。
     * @author Rainy
     * @see --advanced
     */
    [ JsonObject ( MemberSerialization.OptIn)]
    public class BaseModelClientValidationRule 
    {
        [JsonProperty("g")]
        public string[] Groups { get; set; }
        [JsonProperty( "e" )]
        public string ErrorMessage { get; set; }
        [JsonProperty( "p" )]
        public IDictionary<string, object> ValidationParameters { get; }
        [JsonProperty( "t" )]
        public string ValidationType { get; set; }

        public BaseModelClientValidationRule ( string validationType, bool hasParameters )
        {
            if (hasParameters)
            {
                ValidationParameters = new Dictionary<string, object>( );
            }
            ValidationType = validationType;
        }
        public BaseModelClientValidationRule(string validationType,string errorMessage,string[] groups,bool hasParameters )
        {
            if (hasParameters)
            {
                ValidationParameters = new Dictionary<string, object>( );
            }
            ValidationType = validationType;
            ErrorMessage = errorMessage;
            Groups = groups;
        }
    }
}
