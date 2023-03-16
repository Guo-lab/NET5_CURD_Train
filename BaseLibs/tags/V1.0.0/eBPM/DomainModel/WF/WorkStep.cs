using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace eBPM.DomainModel.WF
{
    public class WorkStep : BaseDomainObject
    {
        public static readonly SortStruc<WorkStep>[] DefaultSort = new SortStruc<WorkStep>[]
                                                                    {
                                                                        new SortStruc<WorkStep>{
                                                                            OrderByExpression=o => o.StepNo,
                                                                            OrderByDirection = OrderByDirectionEnum.Asc
                                                                        }
                                                               };
        #region "persistent properties"

        [MappingIgnore]
        private WorkFlow _workflow;
        [DomainSignature]
		public virtual WorkFlow WorkFlow { get { return _workflow; } }

        [MappingIgnore]
        private int _stepNo;
		[DomainSignature]
		public virtual int StepNo { get { return _stepNo; } }
		public virtual string Name { get; set; }
		
		public virtual StepCategoryEnum StepCategory { get; set; }
        
        public virtual string MappedStatus { get; set; }

        public virtual string StepOwner { get; set; }
        public virtual eBPM.Engine.FinishStepModeEnum? FinishStepMode { get; set; }
        [MappingIgnore]
        private Guid _keys;
        public virtual Guid Keys { get { return _keys; } }

        public virtual IList<WorkBranch> Branches { get; set; }

        [MappingIgnore]
        public virtual string NextStep
        {
            get
            {
                if (this.Branches == null || this.Branches.Count ==0)//内存中的对象或结束步骤
                    return "";
                else 
                {
                    return string.Join ("\r" ,this.Branches.Select(x => GetNextStepDisplay(x)));
                }
            }
        }

        private string GetNextStepDisplay(WorkBranch branch)
        {
            string ret = "";
            if (!string.IsNullOrEmpty(branch.Action))
                ret += branch.Action + ":";
            else if (!string.IsNullOrEmpty(branch.Expression))
                ret += branch.Expression + ":";

            ret += branch.NextStep.StepNo + "-" + branch.NextStep.Name;
            return ret;
        }
        #endregion


        #region "methods"
        public virtual void NewWorkStep(WorkFlow wf, int stepNo)
        {

            SetStepNo(stepNo);
            _workflow = wf;
            _keys = Guid.NewGuid();
        }
        public virtual void NewWorkStep(WorkFlow wf, WorkStep from)
        {
            SetStepNo(from.StepNo);
            _workflow = wf;
            _keys = from.Keys;
        }

        private void SetStepNo(int stepNo)
        {
            _stepNo = stepNo;
        }
        public virtual void StepNo_Refresh(int step)
        {
            _stepNo = _stepNo + step;
        }

        #endregion

        #region "Enums"

        #endregion
    }

}

