using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.Web.Mvc
{
    public abstract class BaseFieldControlAttribute : Attribute
    {
        public string? FieldName { get; set; }
        public string? ListName { get; set; }
        public bool? ReadOnly { get; set; }
        public BaseFieldControlAttribute(string? fieldCode, string? ListName, bool? flag)
        {
            this.FieldName = fieldCode;
            this.ListName = ListName;   
            this.ReadOnly = flag;
        }
    }
}
