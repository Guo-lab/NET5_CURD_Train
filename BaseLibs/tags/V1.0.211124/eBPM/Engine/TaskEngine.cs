using eBPM.DomainModel;
using eBPM.DomainModel.WF;
using eBPM.DomainModel.WorkFlowObject;
using eBPM.Exception;
using eBPM.Param;
using eBPM.Role;
using eBPM.Service;
using ESC5.Job.Message;
using ExpressionEvaluator;
using IdentityService;
using ProjectBase.Domain.Transaction;
using ProjectBase.MessageSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace eBPM.Engine
{
    public abstract class TaskEngine<T, ProcessorT> : IDisposable where T : BaseProcess, new()
                                                               where ProcessorT : BaseCurrentProcessor, new()
    {
        public IMessageSender<JobOrder> MessageSender
        {
            get; set;
        }
       
        public IParamValueFinderFactory pvfFactory { get; set; }
        public IProcessorFinderFactory pfFactory { get; set; }
        public ITransactionHelper TransactionHelper { get; set; }

        public IIdentity Identity { get; set; }
        public event BeforeStartNextStepHandler BeforeStartNextStep;

        public BPMContext Context { get; set; }
        protected BaseWorkFlowObject<T, ProcessorT> WorkFlowObject
        {
            get
            {
                if (this.Context != null)
                    return this.Context.WorkFlowObject as BaseWorkFlowObject<T, ProcessorT>;
                else
                    return null;
            }
        }

        /// <summary>
        /// 根据processType字符串得到此步骤对应的ProcessTypeEnum
        /// 基于约定，如果processType不包含".",则以T的名称+TypeEnum作为Enum名称
        /// 否则会提供完整的枚举类型,如MyProcessTypeEnum.Publish
        /// 默认枚举应该与T在一个 Assembly 中，如果不在一起，应该加上 ,AssemblyName
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        protected int?  GetProcessType(string processType)
        {
            string assemblyName = GetAssemblyName(processType);
            string enumType = GetEnumType(processType, "TypeEnum");
            string processTypeName = GetEnumString(processType);
            return GetEnumValue(assemblyName, enumType, processTypeName);
        }

        /// <summary>
        /// 根据Status字符串得到当前步骤对应的StatusEnum
        /// 基于约定，如果status不包含".",则以T的名称去掉Process+StatusEnum作为Enum名称
        ///否则会提供完整的枚举类型,如MyStatusEnum.Approved
        /// 默认枚举应该与T在一个 Assembly 中，如果不在一起，应该加上 ,AssemblyName
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected int? GetStatus(string status)
        {
            string assemblyName = GetAssemblyName(status);
            string enumType = GetEnumType(status, "StatusEnum", "Process");
            string statusName = GetEnumString(status);
            return GetEnumValue(assemblyName, enumType, statusName);
        }

        /// <summary>
        /// 根据字符串得到对应的动作之后此步骤的ProcessResultEnum
        /// 基于约定，此字段可以有3种格式
        ///      1 如果为空，则对应ProcessResultEnum.None
        ///      2 Result:Action
        ///      3 Action 等同于None:Action
        /// 如果Result不包含".",则以ProcessResultEnum作为Enum名称
        ///否则会提供完整的枚举类型,如CustomProcessResultEnum.Approved
        /// 默认枚举应该与T在一个 Assembly 中，如果不在一起，应该加上 ,AssemblyName
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected int GetResult(string result)
        {
            if (string.IsNullOrEmpty(result))
                return 0; //None
            try
            {
                string[] tmp = result.Split(':');
                if (tmp.Length > 1)
                {
                    string assemblyName = GetAssemblyName(tmp[0]);
                    string enumType;
                    if (tmp[0].Contains(".") == false && tmp[0].Contains(",") == false) {
                        enumType = "ProcessResultEnum";
                    }
                    else
                    {
                        enumType=GetEnumType(tmp[0], "ProcessResultEnum", "Process");
                    }
                    string resultValue = GetEnumString(tmp[0]);
                    int? value = GetEnumValue(assemblyName, enumType, resultValue);
                    if (value.HasValue)
                    {
                        return value.Value;
                    }
                    else
                    {
                        throw new ProcessResultNotDefinedException(result);
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                throw new ProcessResultNotDefinedException(result);
            }
        }


        private string GetAssemblyName(string fullName) {
            int index = fullName.LastIndexOf(",");
            if (index == -1)
            {
                return typeof(T).Assembly.FullName;
            }
            else {
                return fullName.Substring(index+1).Trim();
            }
        }
        
        private string GetEnumType(string fullName, string defaultSuffix, string replace="")
        {
            string s = fullName;

            int index = fullName.LastIndexOf(",");
            if (index >= 0)
            { s = fullName.Substring(0, index); }

            index = s.LastIndexOf(".");
            if (index == -1)
            {
                if (replace == "")
                {
                    return typeof(T).Name + defaultSuffix;
                }
                else
                {
                    return typeof(T).Name.Replace(replace, "") + defaultSuffix;
                }
            }
            else {
                return s.Substring(0, index).Trim();
            }
        }
        private string GetEnumString(string fullName)
        {
            if (fullName.Contains(".") == false && fullName.Contains(",") == false) { return fullName; }

            string s = fullName;

            int index = fullName.LastIndexOf(",");
            if (index >= 0)
            { s = fullName.Substring(0, index); }

            index = s.LastIndexOf(".");
            if (index < 0)
            { return s; }
            else { return s.Substring(index+1).Trim (); }           

        }

        private int? GetEnumValue(string assemblyName,string enumName,string enumValue)
        {
            Type type = Assembly.Load (assemblyName ).GetType(enumName);
            try
            {
                return (int)Enum.Parse(type, enumValue);
            }
            catch
            {
                return null;
            }
        }

        public virtual void Dispose()
        {
            if (Context != null)
                Context = null;
            if (BeforeStartNextStep != null)
                BeforeStartNextStep = null;
        }
        
        //确定当前用户的任务完成后，是否可以进入下一个步骤
        protected bool CanGotoNextStep(WorkStep step)
        {
            if (!step.FinishStepMode.HasValue)
            {
                return true; //只有处理步骤才有FinishStepMode，其他类型不应该调用此方法
            }
            switch (step.FinishStepMode.Value)
            {
                case FinishStepModeEnum.AllUser:
                    return WorkFlowObject.Processor.Count == 0;
                case FinishStepModeEnum.OneUser:
                    
                    return true;
                default:
                    if (BeforeStartNextStep != null)
                        return BeforeStartNextStep(Context);
                    else
                        return true;
            }
        }
        #region "Set Next Step"

        protected WorkStep GetNextStep(WorkStep currentStep)
        {
            foreach (WorkBranch branch in currentStep.Branches)
            {
                if (IsBranchSatisfied(branch))
                {
                    if (branch.NextStep.StepCategory == StepCategoryEnum.Branch)
                        return GetNextStep(branch.NextStep);
                    else
                        return branch.NextStep;
                }
            }
            throw new NoBranchSatisfiedException(currentStep.Name);
        }
        protected void SetNextStep(WorkStep step)
        {
            if (!CanGotoNextStep(step)) return;
            WorkFlowObject.ClearProcessor();
            WorkStep nextStep = GetNextStep(step);

            int? status = GetStatus(nextStep.MappedStatus);
            if (status == null)
                throw new InvalidStatusException(nextStep.MappedStatus);
            WorkFlowObject.Status = status.Value ;
            this.WorkFlowObject.CurrentStep = nextStep;

            if (nextStep.StepCategory == StepCategoryEnum.Process)
            {
                IList<IUser> processors = GetStepOwner(nextStep.StepOwner);
                if (processors.Count == 0)
                    throw new ProcessorNotFoundException(nextStep.Name);
                this.AddProcessor(processors);

            }
            else if (nextStep.StepCategory == StepCategoryEnum.Start)
            {
                //退回到Issuer
                this.AddProcessor(WorkFlowObject.GetIssuer());
            }
            else //结束步骤
            {

            }
        }


        private void AddProcessor(IList<IUser> processors)
        {
            foreach (IUser processor in processors)
            {
                WorkFlowObject.AddProcessor(new ProcessorT { Processor = processor,ReceivedTime=DateTime.Now });
            }
        }

        private IList<IUser> GetStepOwner(string stepOwner)
        {
            IProcessorFinder finder = pfFactory.CreateProcessorFinder(stepOwner);
            return finder.FindProcesssor(this.Context);
        }

        protected bool IsBranchSatisfied(WorkBranch branch)
        {
            if (!string.IsNullOrEmpty(this.Context.Action) && !string.IsNullOrEmpty(branch.Action))
            {
                //Branch.Action的格式为ProcessResult:Action或者Action，后者等同于None:Action
                if (branch.Action.IndexOf(":") != -1)
                    return branch.Action.Split(':')[1] == this.Context.Action;
                else
                    return branch.Action == this.Context.Action;
            }

            if (string.IsNullOrEmpty(branch.Action) && string.IsNullOrEmpty(branch.Expression))
                return true;

            if (!string.IsNullOrEmpty(branch.Expression))
            {
                string translatedExpression = TranslateExpression(branch.Expression);
                try
                {
                    return Calculator.CalculateBoolean(translatedExpression);
                }
                catch (System.Exception ex)
                {
                    throw new ExpressionCompileException(ex.Message);
                }
            }
            return false;

        }

        private string TranslateExpression(string expression)
        {
            MatchCollection mc = Regex.Matches(expression, "@[A-Z]{1}[A-Za-z]{0,18}", RegexOptions.IgnoreCase);
            IList<Parameter> parameters = this.WorkFlowObject.CurrentStep.WorkFlow.Parameters;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (Match match in mc)
            {
                Parameter parameter = parameters.FirstOrDefault(x => x.ParameterName == match.Value);
                if (parameter == null)
                    throw new ParameterNotDefinedException(match.Value);

                object value = GetParameterValue(parameter);
                sb.Append(" " + parameter.ParameterName + "=" + value.ToString());
                expression = expression.Replace(parameter.ParameterName, FormatValue(parameter.ParameterType.Value, value).ToString());
            }
            
            return expression;
        }

        private object FormatValue(ValueTypeEnum parameterType, object value)
        {
            switch (parameterType)
            {
                case ValueTypeEnum.Numeric:
                    return value;
                case ValueTypeEnum.String:
                    return "'" + value.ToString() + "'";
                case ValueTypeEnum.DateTime:
                    return "'" + value.ToString() + "'";
                case ValueTypeEnum.YesNo:
                    return value;
                default:
                    return value;
            }
        }

        private object GetParameterValue(Parameter parameter)
        {
            if (!string.IsNullOrEmpty(parameter.MapToProperty))
            {
                return GetPropertyValue(this.WorkFlowObject, parameter.MapToProperty);
            }
            else
            {
                IParamValueFinder finder = pvfFactory.CreateParameterValueFinder(parameter.ParameterValueFinder);
                return finder.FindParameterValue(this.Context);
            }

        }

        private object GetPropertyValue(object o, string propertyname)
        {
            string[] properties = propertyname.Split(new char[] { '.' });
            System.Reflection.PropertyInfo PI = null;
            object obj = o;
            foreach (string s in properties)
            {
                bool haserror = true;
                if (obj == null)
                    break;
                Type tp = obj.GetType();
                while (haserror)
                {
                    try
                    {
                        PI = tp.GetProperty(s);
                        haserror = false;
                    }
                    catch
                    {
                        tp = tp.BaseType;
                    }
                }

                obj = PI.GetValue(obj, null);
            }

            return obj;
        }
        #endregion

        private string _entityName = "";
        protected virtual string GetEntityName()
        {
            if (_entityName == "") { _entityName = typeof(T).Name.Replace("Process", ""); }
            return _entityName;
        }
        protected virtual void SendJobOrderMessage()
        {
            string orderId = WorkFlowObject.Id.ToString();
            TransactionHelper.AddAsyncTask(() =>
            {
                MessageSender.Send(new JobOrder
                {
                    Id = orderId,
                    OrderType = this.GetEntityName()
                });
            });
        }
        private WorkFlow _workFlow;
        protected virtual WorkFlow GetWorkFlow(string entityName) {
            if (_workFlow == null)
            {
                _workFlow = WorkFlowService.GetLatestVersion(entityName);
            }
            return _workFlow;
        }
        public IWorkFlowService WorkFlowService { get; set; }

    }
}
