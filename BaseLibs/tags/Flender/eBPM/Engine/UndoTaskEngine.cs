using System;
using System.Linq;
using eBPM.DomainModel.WorkFlowObject;
using eBPM.Exception;
using eBPM.DomainModel.WF;
using System.Collections.Generic;
namespace eBPM.Engine
{
    //TaskEngine的客户端应该把TaskEngine包含在using中或者最后调用Dispose方法
    public class UndoTaskEngine<T, ProcessorT> : TaskEngine<T, ProcessorT>, IUndoTaskEngine<T, ProcessorT> where T : BaseProcess, new()
                                                                                                                where ProcessorT : BaseCurrentProcessor, new()
    {

        public event BeforeUndoTaskHandler BeforeUndoTask;
        public event AfterUndoTaskHandler AfterUndoTask;

        public UndoTaskEngine()
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            if (BeforeUndoTask != null)
                BeforeUndoTask = null;
            if (Context != null)
                Context = null;
        }

        private bool IsDuringMyProcess()
        {
            //缺省实现：找到当前用户的最后一条处理记录，查看此记录后面是否有其他用户的处理记录
            T lastProcess = WorkFlowObject.Process.LastOrDefault(x => x.Processedby.Id == Identity.GetWorkingUserId());
            if (lastProcess == null)
            {
                return false;
            }
            bool canUndo;
            if (lastProcess.WorkStep.FinishStepMode == FinishStepModeEnum.AllUser)
            {
                //会签模式下，不存在其它步骤的Process记录
                canUndo = WorkFlowObject.Process.Count(x => x.Id > lastProcess.Id && x.WorkStep.Id != lastProcess.WorkStep.Id) == 0;
            }
            else
            {
                //非会签模式，不存在后续的步骤
                canUndo = WorkFlowObject.Process.Count(x => x.Id > lastProcess.Id) == 0;
            }

            if (BeforeUndoTask != null) 
                //用户可自定义是否能撤销。
                //某些条件下即使满足缺省条件也不能撤销，例如订单已盖章
            {
                return BeforeUndoTask(Context) && canUndo;
            }
            else {
                return canUndo;
            }
        }


        #region "Engine"

        public virtual void UndoTask()
        {
            if (!IsDuringMyProcess())
                throw new NotUnderProcessingException();
            T lastProcess = WorkFlowObject.Process.LastOrDefault(x => x.Processedby.Id == Identity.GetWorkingUserId());
            RestoreStepTo(lastProcess);
            if (AfterUndoTask != null)
            {
                AfterUndoTask(this.Context);
            }
            //this.Dispose();
            CalculatePendingJob();
        }

        private void RestoreStepTo(T process)
        {
            //清除后续步骤的Processor记录
            if (process.WorkStep.FinishStepMode!= FinishStepModeEnum.AllUser) //在会签状态下，只一个用户签字完成整个单据的Step并未改变，此时不需清除Processor集合
            {
                WorkFlowObject.Processor.Clear();
            }
            //设置WorkFlowObject的CurrentStep和Status
            int? status = GetStatus(process.WorkStep.MappedStatus);
            if (status == null)
                throw new InvalidStatusException(process.WorkStep.MappedStatus);
            WorkFlowObject.Status = status.Value;
            this.WorkFlowObject.CurrentStep = process.WorkStep;
            //添加Undo用户Processor记录
            WorkFlowObject.AddProcessor(new ProcessorT { Processor = process.Processedby,ReceivedTime=process.ReceivedTime });
            //删除Undo用户的Process记录
            WorkFlowObject.Process.Remove(process);
        }
        #endregion
    }
}

