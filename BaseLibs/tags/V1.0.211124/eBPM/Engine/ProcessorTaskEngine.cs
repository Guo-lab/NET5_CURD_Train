using eBPM.DomainModel.WF;
using eBPM.DomainModel.WorkFlowObject;
using eBPM.Exception;
using System;
using System.Linq;
namespace eBPM.Engine
{
    //TaskEngine的客户端应该把TaskEngine包含在using中或者最后调用Dispose方法
    public class ProcessorTaskEngine<T, ProcessorT> : TaskEngine<T, ProcessorT>, IProcessorTaskEngine<T, ProcessorT> where T : BaseProcess, new()
                                                                                                                where ProcessorT : BaseCurrentProcessor, new()
    {

        public event BeforeProcessTaskHandler BeforeProcessTask;
        public event AfterProcessTaskHandler AfterProcessTask;

        public ProcessorTaskEngine()
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            if (BeforeProcessTask != null)
            {
                BeforeProcessTask = null;
            }

            if (AfterProcessTask != null)
            {
                AfterProcessTask = null;
            }
        }

        private bool IsDuringMyProcess()
        {
            if (BeforeProcessTask != null)
            {
                return BeforeProcessTask(Context);
            }
            else
            {
                //缺省实现：Processor中存在当前用户的未处理记录
                return WorkFlowObject.Processor.Count(x => x.Processor.Id == Identity.GetWorkingUserId()) > 0;
            }
        }


        #region "Engine"

        public virtual void ProcessTask()
        {
            if (!IsDuringMyProcess())
            {
                throw new NotUnderProcessingException();
            }

            FinishCurrentStep();
            SetNextStep(this.WorkFlowObject.CurrentStep);
            if (AfterProcessTask != null)
            {
                AfterProcessTask(Context);
            }

            //this.Dispose();
            SendJobOrderMessage();
        }
        #endregion


        #region "Finish Current Step"
        private void FinishCurrentStep()
        {
            WorkBranch branch = WorkFlowObject.CurrentStep.Branches.FirstOrDefault(x => IsBranchSatisfied(x));
            if (branch == null)
            {
                throw new NoBranchSatisfiedException(WorkFlowObject.CurrentStep.Name);
            }

            if (Context.CurrentProcessor != null && !string.IsNullOrEmpty(branch.MappedProcessType))
            {
                object processType = GetProcessType(branch.MappedProcessType);
                if (processType == null)
                {
                    throw new ProcessTypeNotDefinedException(branch.MappedProcessType);
                }

                ProcessorT processor = WorkFlowObject.Processor.FirstOrDefault(x => x.Processor.Id == Context.CurrentProcessor.Id);
                DateTime receivedTime = processor == null ? DateTime.Now : processor.ReceivedTime;

                this.WorkFlowObject.AddProcess(new T
                {
                    WorkStep = WorkFlowObject.CurrentStep,
                    Processedby = Context.CurrentProcessor,
                    Acting = Context.ActingProcessor,
                    ProcessedTime = DateTime.Now,
                    ReceivedTime = receivedTime,
                    ProcessType = Convert.ToInt32(processType),
                    ProcessResult = GetResult(branch.Action),
                    Comments = Context.Comments
                });

                WorkFlowObject.RemoveProcessor(Context.CurrentProcessor);
            }
        }

    }
    #endregion
}

