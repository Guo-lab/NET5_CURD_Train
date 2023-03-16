using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESC5.ValueObject;

namespace ESC5.Domain.Base.QR
{
    public interface IQRItem
    {
        int ItemNo { get; set; }

        string? PartNo { get; set; }
        string Description { get; set; }

        UnitVO Unit { get; set; }
        decimal Quantity { get; set; }
    }
}
