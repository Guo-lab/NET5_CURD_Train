using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLibrary.Validate
{
    public class OrderAttribute:Attribute
    {
        public int ValidateOrder { get; set; }
        public OrderAttribute (int order)
        {
            this.ValidateOrder = order;
        }
    }
}
