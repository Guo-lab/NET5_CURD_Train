using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBase.BusinessDelegate
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ValidatorAttribute : Attribute
    {
        private readonly string _attribute;

        public string attribute
        {
            get { return _attribute; }
        }

        public ValidatorAttribute(string attribute)
        {
            _attribute = attribute;
        }
    }
}
