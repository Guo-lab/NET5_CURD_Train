using Newtonsoft.Json;
using ProjectBase.Web.Mvc.Validation;
using System;
using System.Collections.Generic;

namespace ProjectBase.Web.Mvc.Tool
{
    /**
     * provide Json format object of metadata to client
     * @see --internal
     */

    public class ClientVmMetadata {

        private string _modelType;

        [JsonProperty(PropertyName ="m")]
        public string ModelType {
            get{
                return _modelType;
            }
            set{
                if (value.Equals( "ListInput", StringComparison.OrdinalIgnoreCase )){
                    value = "ListInput";
                }
                _modelType = value;
            } }
        [JsonProperty( PropertyName = "v" )]
        public IDictionary<string,BaseModelClientValidationRule> ValidationRules { get; set; }
        [JsonProperty( PropertyName = "f" )]
        public bool? FormIgnore { get; set; }
        [JsonProperty( PropertyName = "s" )]
        public bool? SubmitIgnore { get; set; }
        [JsonProperty( PropertyName = "c" )]
        public IDictionary<string, ClientVmMetadata> Properties { get; set; }
        [JsonProperty( PropertyName = "r" )]
        public bool IsRequired { get; set; }
        [JsonProperty( PropertyName = "k" )]
        public string TranslateKey { get; set; }

        public void SetValidationRules ( IEnumerable<BaseModelClientValidationRule> rules ) {
            ValidationRules = null;
            AddValidationRules( rules );
        }
        public void AddValidationRules ( IEnumerable<BaseModelClientValidationRule> rules ) {
            if (rules == null || !rules.GetEnumerator().MoveNext( )) return;
            if (ValidationRules == null) {
                ValidationRules = new Dictionary<String, BaseModelClientValidationRule>( );
            }
			foreach(BaseModelClientValidationRule rule in rules) {
                ValidationRules[rule.ValidationType] = rule;
            }
        }
        //为叶型数组或集合加整体验证。因为对应meta节点保存了元素meta包括rules，所以整体验证保存与元素验证一处保存时需要使用validationType来区分
        public void AddLeafEleValidationRules ( IEnumerable<BaseModelClientValidationRule> rules ) {
            if (rules == null || !rules.GetEnumerator( ).MoveNext( )) return;
            if (ValidationRules == null) {
                ValidationRules = new Dictionary<string, BaseModelClientValidationRule>( );
            }
            foreach (BaseModelClientValidationRule rule in rules)
            {
                ValidationRules.Add( GlobalConstant.VmMeta_LeafElePrefix + rule.ValidationType, rule );
            }
        }
    }
}
