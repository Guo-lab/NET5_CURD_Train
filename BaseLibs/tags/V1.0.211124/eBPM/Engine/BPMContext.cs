using System;
using System.Collections.Generic;
using System.Linq;
using eBPM.DomainModel;
using eBPM.Role;

namespace eBPM.Engine
{
    public enum FinishStepModeEnum
    {
        AllUser = 1, //所有用户完成才进入下一步
        OneUser, //一个用户完成即进入下一步
        Custom //定制
    }
    public class BPMContext
    {
        public object WorkFlowObject { get; set; }
        /// <summary>
        /// Processed by
        /// </summary>
        public IUser CurrentProcessor { get; set; } //当前任务的Processor
        public IUser ActingProcessor { get; set; }  //代理用户

        public FinishStepModeEnum FinishStepMode { get; set; }
        public string Comments { get; set; }

        public string Action { get; set; }


    }
}
