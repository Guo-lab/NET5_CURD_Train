using Microsoft.AspNetCore.Http;
using System;

namespace ProjectBase.Web.Mvc
{
    /**
	 * Action方法上的标记处理器基类。
	 * @author Rainy
	 * @see --advanced
	 */
    public abstract class AttributeParserAsFilter<TAttribute>
    {
        private const string AnnotaionKeyPrefix = "NetArch_AttributeOnAction_";

        protected TAttribute? GetAttribute(HttpContext httpContext)
        {
            return (TAttribute)httpContext.Items[AnnotaionKeyPrefix + typeof(TAttribute).FullName];
        }
        protected void SetAttribute(HttpContext httpContext, Attribute an)
        {
            httpContext.Items[AnnotaionKeyPrefix + typeof(TAttribute).FullName] = an;
        }
        protected TAttribute? GetAttributeOnController(HttpContext httpContext)
        {
            return (TAttribute)httpContext.Items[AnnotaionKeyPrefix +"Controller_"+ typeof(TAttribute).FullName];
        }
        protected void SetAttributeOnController(HttpContext httpContext, Attribute an)
        {
            httpContext.Items[AnnotaionKeyPrefix + "Controller_" + typeof(TAttribute).FullName] = an;
        }
        /**
         * when the annotation is absent from action,whether to use annotation on controller class as default.
         * @return
         */
        public virtual bool ShouldParseControllerDefault()
        {
            return false;
        }

    }
}
