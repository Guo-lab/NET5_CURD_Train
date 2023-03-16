using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC5.Domain.Base.IM
{
    public interface IPart
    {
        int Id { get; }
        string PartNo { get; set; }
        string Description { get; set; }
        bool IsActive { get; set; }
        bool StockMaterial { get; set; }

        bool LotEnabled { get; set; } //是否启用批次，如为true,则入库时必须输入批次号
        bool SNEnabled { get; set; }//是否启用序列号，如为true,则入库时必须输入序列号
        bool ShelfLifeEnabled { get; set; } //是否启用保质期，如为true入库时必须输入保质期截止日期
    }
}
