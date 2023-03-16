using System;
using System.Linq;
using eBPM.DomainModel.WF;
using eBPM.DomainModel.WorkFlowObject;
using eBPM.Exception;
using eBPM.DomainModel;
using eBPM.Service;
using ProjectBase.Web.Mvc;

namespace eBPM.Engine
{
    //TaskEngine的客户端应该把TaskEngine包含在using中或者最后调用Dispose方法
    public class IssuerTaskEngine<T,ProcessorT>: TaskEngine<T,ProcessorT>,IIssuerTaskEngine<T,ProcessorT> where T :BaseProcess,new()
                                                                                                          where ProcessorT :BaseCurrentProcessor , new()
    {
        public event BeforeStartTaskHandler BeforeStartTask;
        public event AfterStartTaskHandler AfterStartTask;

        public event BeforeUpdateTaskHandler BeforeUpdateTask;
        public event AfterUpdateTaskHandler AfterUpdateTask;
        
        public IssuerTaskEngine()
        {
            
        }
        public override void Dispose()
        {
            base.Dispose();
            if (BeforeStartTask != null)
                BeforeStartTask = null;
            if (AfterStartTask != null)
                AfterStartTask = null;
            if (BeforeUpdateTask != null)
                BeforeUpdateTask = null;
            if (AfterUpdateTask != null)
                AfterUpdateTask = null;
        }
        
        #region "Start Engine"
        public virtual void StartTask()
        {
            if (!CanStart())
                throw new DuplicatedRequestException();

            string entityName = GetEntityName();
            WorkFlow workFlow = GetWorkFlow(entityName); 
            
            if (workFlow == null)
                throw new NoWorkFlowDefinedException();
            WorkStep currentStep = workFlow.Steps.First();
            WorkFlowObject.CurrentStep = currentStep;
            DateTime currentTime = DateTime.Now;
            WorkFlowObject.LastModifiedTime = currentTime;

            WorkBranch branch = WorkFlowObject.CurrentStep.Branches.Where(b => b.Action.Contains("Request")).FirstOrDefault();

            int? processType = GetProcessType(branch.MappedProcessType);

            if (processType.HasValue == false )
                throw new ProcessTypeNotDefinedException(branch.MappedProcessType);

            if (Context.CurrentProcessor != null)
            { //系统自动驱动的Flow不加Process
                this.WorkFlowObject.AddProcess(new T
                {
                    WorkStep = currentStep,
                    Processedby = Context.CurrentProcessor,
                    Acting = Context.ActingProcessor,
                    ProcessedTime = currentTime,
                    ReceivedTime=currentTime,
                    ProcessType = processType.Value,
                    ProcessResult = 1 //None
                });
            }
            
            SetNextStep(currentStep);
            if (AfterStartTask != null)
                AfterStartTask(Context);

            //this.Dispose();
            SendJobOrderMessage();
        }

        private bool CanStart()
        {
            //可以根据需要验证不要重复提交，判断的依据一般是相邻若干秒之内某些字段的值完全相同
            if (BeforeStartTask != null)
                return BeforeStartTask(Context);
            else
                return true;
        }
        #endregion
        #region "Update Task"
        public virtual void UpdateTask()
        {
            if (!CanUpdate())
                throw new NotUnderProcessingException();
            DateTime currentTime = DateTime.Now;
            WorkFlowObject.LastModifiedTime = currentTime;
            //重置到第一步
            WorkFlowObject.CurrentStep = WorkFlowObject.CurrentStep.WorkFlow.Steps.First();
            int? processType = GetProcessType("Update");
            if (processType == null)
                throw new ProcessTypeNotDefinedException("Update");

            if (Context.CurrentProcessor != null)
            {
                //系统自动驱动的Flow不添加process
                this.WorkFlowObject.AddProcess(new T
                {
                    WorkStep = WorkFlowObject.CurrentStep,
                    Processedby = Context.CurrentProcessor,
                    Acting = Context.ActingProcessor,
                    ProcessedTime = currentTime,
                    ReceivedTime = currentTime,
                    ProcessType = processType.Value,
                    ProcessResult = 1, //None
                    Comments = Context.Comments
                });
            }

            WorkFlowObject.ClearProcessor();

            SetNextStep(WorkFlowObject.CurrentStep);
            if (AfterUpdateTask != null)
                AfterUpdateTask(Context);

            //this.Dispose();

            SendJobOrderMessage();
        }
                

        private bool CanUpdate()
        {
            //提供定制化的验证逻辑，保证当前状态可以更新
            if (BeforeUpdateTask != null)
                return BeforeUpdateTask(Context);
            else
                //缺省实现 Issuer包含登陆用户 且 当前步骤位于开始步骤或者最后一次提交后没有用户处理
                return WorkFlowObject.GetIssuer().Select(x=>x.Id).Contains (Identity.GetWorkingUserId ()) && 
                    (WorkFlowObject.CurrentStep.StepCategory==StepCategoryEnum.Start ||
                     WorkFlowObject.Process.Count(x=>x.ProcessedTime>WorkFlowObject.LastModifiedTime)==0
                    );
        }
        #endregion
        
    }
}
