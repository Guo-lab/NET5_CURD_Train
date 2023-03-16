using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using ProjectBase.Utils;

namespace ProjectBase.Web.Mvc.Angular
{
    /// <summary>
    /// 
    /// 定制meta数据，主要是默认TranslateKey
    /// </summary>
    public class CustomModelMetadataProvider : DefaultModelMetadataProvider
    {
        public CustomModelMetadataProvider(ICompositeMetadataDetailsProvider detailsProvider)
            : base(detailsProvider)
        {
        }
        public CustomModelMetadataProvider(ICompositeMetadataDetailsProvider detailsProvider, IOptions<MvcOptions> optionsAccessor)
    : base(detailsProvider, optionsAccessor)
        {
        }

        protected override ModelMetadata CreateModelMetadata(DefaultMetadataDetails entry)
        {
            DefaultModelMetadata metadata = (DefaultModelMetadata)base.CreateModelMetadata(entry);
            if (metadata.MetadataKind == ModelMetadataKind.Property)
            {
                metadata.DisplayMetadata.ConvertEmptyStringToNull = false;
            }

            //---customize display name 
            if (!string.IsNullOrEmpty(metadata.DisplayName)) return metadata;

            var attrs = entry.ModelAttributes.Attributes;
            var propertyName = metadata.PropertyName;
            var containerType = metadata.ContainerType;

            if (propertyName == null && containerType == null && attrs.OfType<DisplayNameKeyAttribute>().FirstOrDefault()==null) return metadata;
            if (containerType != null) {
                var ns = ProjectHierarchy.ViewModelNS.Split(',');
                if (ns.Where((n) => containerType.Namespace!=null && containerType.Namespace.Contains(n)).FirstOrDefault() == null) return metadata;
            }

            var customDisplayName = DisplayNameForProp(attrs, containerType, propertyName);

            if (string.IsNullOrEmpty(customDisplayName) && propertyName != null && propertyName.EndsWith(DisplayExtension.SurfixForDisplay) && containerType != null)
            {
                customDisplayName = DisplayNameForProp(null, containerType, propertyName.Replace(DisplayExtension.SurfixForDisplay, ""));
            }
            if (string.IsNullOrEmpty(customDisplayName))
            {
                customDisplayName = null;//important!don't let DisplayName be empty,make it null insteadof empty.Otherwise ValidationContext.set_DisplayName would throw exception
            }

            metadata.DisplayMetadata.DisplayName = () => customDisplayName;
            return metadata;
        }

        private string DisplayNameForProp(IReadOnlyList<object> attributes, Type containerType, string propertyName)
        {
            string resourceName = "";
            DisplayNameKeyAttribute keyattr = null;
            if (attributes != null)
                keyattr = attributes.OfType<DisplayNameKeyAttribute>().FirstOrDefault();
            else
            {
                keyattr = (DisplayNameKeyAttribute)Attribute.GetCustomAttribute(containerType.GetProperty(propertyName), typeof(DisplayNameKeyAttribute));
            }

            if (keyattr != null)
            {
                return keyattr.Key;
            }

            if (containerType != null)
            {
                var classAttribute =
                    Attribute.GetCustomAttribute(containerType, typeof(DisplayNameAttribute)) as
                    DisplayNameAttribute;
                if (classAttribute != null)
                    resourceName = classAttribute.DisplayName + "_";
            }
            if (propertyName != null) resourceName = resourceName + propertyName;
            return resourceName;
        }

    }
}
