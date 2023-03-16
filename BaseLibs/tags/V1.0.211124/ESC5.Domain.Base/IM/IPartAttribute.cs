using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Domain.Base.IM
{
    public interface IPartAttribute
    {
        string PartAttrName { get; set; }
        string PartAttrValue { get; set; }
        int AttributeId { get; set; }
        bool IsFixed { get; set; }
        bool IsRequired { get; set; }
    }
}
