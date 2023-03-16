using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eBPM.DomainModel.WorkFlowObject;

namespace eBPM.Engine
{
    public delegate bool BeforeStartTaskHandler(BPMContext context);
    public delegate void AfterStartTaskHandler(BPMContext context);

    public delegate bool BeforeUpdateTaskHandler(BPMContext context);
    public delegate void AfterUpdateTaskHandler(BPMContext context);

    public delegate bool BeforeProcessTaskHandler(BPMContext context);

    public delegate bool BeforeStartNextStepHandler(BPMContext context);
    public delegate void AfterProcessTaskHandler(BPMContext context);

    public delegate bool BeforeUndoTaskHandler(BPMContext context);
    public delegate void AfterUndoTaskHandler(BPMContext context);
    public interface IIssuerTaskEngine
    {
        BPMContext Context { get; set; }
        event BeforeStartTaskHandler BeforeStartTask;
        event AfterStartTaskHandler AfterStartTask;
        void StartTask();

        event BeforeUpdateTaskHandler BeforeUpdateTask;
        event AfterUpdateTaskHandler AfterUpdateTask;
        void UpdateTask();

        event BeforeStartNextStepHandler BeforeStartNextStep;
    }
    public interface IIssuerTaskEngine<T,ProcessorT> :IIssuerTaskEngine where T : BaseProcess,new()
                                                    where ProcessorT: BaseCurrentProcessor, new()
    {
        
    }

    
    public interface IProcessorTaskEngine
    {
        BPMContext Context { get; set; }
        void ProcessTask();
        event BeforeProcessTaskHandler BeforeProcessTask;
        event AfterProcessTaskHandler AfterProcessTask;
        event BeforeStartNextStepHandler BeforeStartNextStep;
    }
    public interface IProcessorTaskEngine<T,ProcessorT> : IProcessorTaskEngine where T : BaseProcess,new()
                                                        where ProcessorT:BaseCurrentProcessor , new()
    {
        
    }

    

    public interface IUndoTaskEngine
    {
        BPMContext Context { get; set; }
        void UndoTask();
        event BeforeUndoTaskHandler BeforeUndoTask;
        event AfterUndoTaskHandler AfterUndoTask;
    }
    public interface IUndoTaskEngine<T, ProcessorT> : IUndoTaskEngine where T : BaseProcess, new()
                                                        where ProcessorT : BaseCurrentProcessor, new()
    {

    }
}
