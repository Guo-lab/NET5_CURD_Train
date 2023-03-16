using eBPM.DomainModel.WF;
using eBPM.Exception;
using eBPM.Role;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
namespace eBPM.DomainModel.WorkFlowObject
{
    [MappingIgnore]
    public abstract class BaseWorkFlowObject<ProcessT, ProcessorT> : VersionedDomainObject where ProcessT : BaseProcess
                                                                                        where ProcessorT : BaseCurrentProcessor
    {
        public virtual WorkStep CurrentStep { get; set; }

        protected IList<ProcessT> _process;
        public virtual IList<ProcessT> Process { get; protected set; }
        public virtual IList<ProcessorT> Processor { get; protected set; }
        //当前单据负责人
        public virtual IUser RequestOwner { get; set; }
        public virtual DateTime LastModifiedTime { get; set; }
        public abstract void AddProcess(ProcessT process);

        public abstract void AddProcessor(ProcessorT processor);


        private Type _statusEnumType;
        public BaseWorkFlowObject()
        {
            string[] domainAssembly = ProjectBase.Utils.ProjectHierarchy.DomainNS.Split(',');
            foreach (string assembly in domainAssembly)
            {
                _statusEnumType = Type.GetType(typeof(ProcessT).Name.Replace("Process", "") + "StatusEnum," + assembly);
                if (_statusEnumType != null)
                {
                    break;
                }
            }
        }
        private int status;
        public virtual int Status
        {
            get
            {
                return status;
            }
            set
            {
                if (_statusEnumType == null)
                {
                    throw new InvalidStatusException(value.ToString());
                }
                if (!Enum.IsDefined(_statusEnumType, value))
                {
                    throw new InvalidStatusException(value.ToString());
                }
                else
                {
                    status = value;
                }
            }
        }

        public virtual IList<IUser> GetIssuer()
        {
            return new List<IUser> { this.RequestOwner };
        }
        public virtual void ClearProcessor()
        {
            if (this.Processor != null) { this.Processor.Clear(); }
        }

        public virtual void RemoveProcessor(IUser processor)
        {
            if (this.Processor != null && this.Processor.Count > 0)
            {
                ProcessorT p = this.Processor.FirstOrDefault(x => x.Processor != null && x.Processor.Id == processor.Id);
                if (p != null)
                { this.Processor.Remove(p); }
            }

        }

        [MappingIgnore]
        public virtual IList<IUser> PendingProcessor
        {
            get
            {
                if (this.Processor !=null && this.Processor.Count > 0)
                {
                    return this.Processor.Select(x => x.Processor).ToList();
                }
                else
                {
                    return null;
                }
            }
        }
        [MappingIgnore]
        public virtual string ProcessorName
        {
            get
            {
                if (this.PendingProcessor == null)
                {
                    return "";
                }
                else
                {
                    return string.Join(",", this.PendingProcessor.Select(x => x.Name).Distinct());
                }
            }
        }

    }

    [MappingIgnore]
    public abstract class BaseProcess : BaseDomainObject
    {
        public virtual WorkStep WorkStep { get; set; }
        public virtual DateTime ProcessedTime { get; set; }
        public virtual IUser Processedby { get; set; }
        public virtual IUser Acting { get; set; }
        public virtual int ProcessResult { get; set; }
        public virtual string Comments { get; set; }

        public BaseProcess()
        {
            string[] domainAssembly = ProjectBase.Utils.ProjectHierarchy.DomainNS.Split(',');
            foreach (string assembly in domainAssembly)
            {
                _processTypeEnumType = Type.GetType(this.GetType().Name + "TypeEnum," + assembly);
                if (_processTypeEnumType != null)
                {
                    break;
                }
            }
        }
        private Type _processTypeEnumType;
        [MappingIgnore]
        private int processType;
        public virtual int ProcessType
        {
            get { return processType; }
            set
            {
               if (_processTypeEnumType == null)
                {
                    throw new ProcessTypeNotDefinedException(value.ToString());
                }
                if (!Enum.IsDefined(_processTypeEnumType, value))
                {
                    throw new ProcessTypeNotDefinedException(value.ToString());
                }
                else
                {
                    processType = value;
                }
            }
        }

        public virtual DateTime ReceivedTime { get; set; }
    }

    [MappingIgnore]
    public class BaseCurrentProcessor : BaseDomainObject
    {
        public virtual IUser Processor { get; set; }
        public virtual DateTime ReceivedTime { get; set; }
    }
}
