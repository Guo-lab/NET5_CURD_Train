using eBPM.DomainModel.WorkFlowObject;
using eBPM.Role;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESC5.Domain.Base.QR
{
    //QR基础类，每个具体的项目应该继承此类加入特定的属性
    [MappingIgnore]
    public abstract class QRBase<ProcessT, ProcessorT> : BaseWorkFlowObject<ProcessT, ProcessorT> where ProcessT : BaseProcess, new()
                                                                                              where ProcessorT : BaseCurrentProcessor
    {
        public virtual string QRNo { get; set; }
        public virtual DateTime QRTime { get; set; }
        public virtual IUser Buyer { get; set; }
        
        public virtual DateTime? StartedTime { get; set; }
        public virtual DateTime? ExpiredTime { get; set; }

    }
    [MappingIgnore]
    public abstract partial class QRBase<ProcessT, ProcessorT, ItemT> : QRBase<ProcessT, ProcessorT> where ProcessT : BaseProcess, new()
                                                                                            where ProcessorT : BaseCurrentProcessor
                                                                                            where ItemT : QRItemBase

    {

        public virtual IList<ItemT> Items { get; protected set; }
    }

}

