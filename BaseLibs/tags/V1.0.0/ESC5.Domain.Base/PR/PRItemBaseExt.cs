using eBPM.DomainModel.WorkFlowObject;
using eBPM.Role;
using System.Linq;
namespace ESC5.Domain.Base.PR
{
    public abstract partial class PRItemBase<ProcessT, ProcessorT> : BaseWorkFlowObject<ProcessT, ProcessorT> where ProcessT : BaseProcess
                                                                                                where ProcessorT : BaseCurrentProcessor
    {
        #region "访问权限"
        /// <summary>
        /// 检查用户是否对此申请有访问权限
        /// 缺省逻辑为申请人或者与申请人同一个主管可以访问
        /// 实际项目可重载此方法，例如当用户部门没有参与到此项目中时，采购员可以访问
        /// </summary>
        public virtual bool HasAccessAuthorityby(IUser user)
        {
            return user.Id == this.RequestOwner.Id || user.ReportTo(this.RequestOwner.Supervisor);
        }

        /// <summary>
        /// 检查用户是否对此申请有处理权限
        /// 缺省逻辑为采购员可以访问
        /// 实际项目可重载此方法，例如同组的采购员不经授权直接可以访问
        /// </summary>
        public virtual bool HasProcessAuthorityby(IUser user)
        {
            if (this.BuyerOwner != null)
            {
                return user.Id == this.BuyerOwner.Id;
            }
            else {
                return true;
            }

        }
        #endregion

        #region "状态翻译"
        public abstract bool IsCancelled();
        public abstract bool POGenerated();
        /// <summary>
        /// 最近一次提交后，下一个审批人尚未处理，此时可以重新提交或者取消
        /// </summary>
        /// <returns></returns>
        protected abstract bool NotYetChecked();
        public abstract bool IsDraft();
        public abstract bool IsReturned();
        public abstract bool IsBlocked();
        public abstract bool IsTobeAssigned();
        public abstract bool IsTobeProcessed();
        public abstract bool IsTobeGenerated();
        #endregion

        #region "添加Process"
        protected abstract void AddCancelProcess(IUser user, string comments);
        #endregion

        #region "更新"

        /// <summary>
        /// 可更新的条件
        /// 符合以下三个条件可以更新 1 草稿 2 被退回 3 最后一次提交后尚未审批
        /// 子类可重载此方法
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanbeUpdatedCondition()
        {
            if (this.IsCancelled()) { return false; }
            if (this.IsDraft()) { return true; }
            if (NotYetChecked()) { return true; }
            if(this.IsReturned()) { return true; }
            return false;
        }
        public virtual bool CanbeUpdatedby(IUser user)
        {
            if (HasAccessAuthorityby(user))
            {
                return CanbeUpdatedCondition();
            }
            else
            {
                return false;
            }
        }

        #endregion 

        #region "取消"
        /// <summary>
        /// 可取消的条件
        /// 符合以下2个条件之一可取消 1 被退回 2最后一次提交后尚未审批
        /// 子类可重载此方法，例如不支持取消功能时可直接返回false
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanbeCancelledCondition()
        {
            if (this.IsDraft() || this.IsCancelled()) {
                return false;
            }
            if (NotYetChecked())
            {
                return true;
            }
            else if (this.IsReturned())
            {
                return true;
            }

            return false;
        }
   

        /// <summary>
        /// 检查用户能否取消订单
        /// 实际项目中可在PRItem类中重载此方法
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool CanbeCancelledby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeCancelledCondition();
            }
            return false;
        }

        /// <summary>
        /// 检查用户能否取消订单
        /// 实际项目中可在PRItem类中重载此方法
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool CanbeDeleteddby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return this.IsDraft();
            }
            return false;
        }

        protected abstract void SetCancelStatus();

        /// <summary>
        /// 通过工作流 取消订单
        /// </summary>
        /// <param name="comments"></param>
        public virtual void Cancel()
        {
            SetCancelStatus();
        }

        /// <summary>
        /// 草稿状态直接取消，需要自行添加取消的Process
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="user"></param>
        public virtual void Cancel(string comments, IUser user)
        {
            SetCancelStatus();
            AddCancelProcess(user, comments);
        }
        #endregion

        #region "分配采购员"
        public virtual bool CanbeAssignedby(IUser user)
        {
            return this.IsTobeAssigned() && this.Processor.Count(x => x.Processor.Id == user.Id) > 0;
        }
        public virtual void AssignTo(IUser assignedTo)
        {
            this.Buyer = assignedTo;
            this.BuyerOwner = assignedTo;
        }
        #endregion

        #region "暂停/恢复"

        
        protected virtual bool CanbeBlockedCondition()
        {
            return this.IsTobeProcessed();
        }
        public virtual bool CanbeBlockedby(IUser user)
        {
            if (HasProcessAuthorityby(user))
            {
                return CanbeBlockedCondition();
            }
            return false;
        }
        public virtual bool CanbeReturnPRby(IUser user)
        {
            if (HasProcessAuthorityby(user))
            {
                return CanbeReturnPRCondition();
            }
            return false;
        }
        protected virtual bool CanbeReturnPRCondition()
        {
            return this.IsBlocked() || this.IsTobeProcessed() || this.IsTobeGenerated() || this.IsTobeAssigned();
        }

        protected virtual bool CanbeUnBlockedCondition()
        {
            return this.IsBlocked();
        }
        public virtual bool CanbeUnBlockedby(IUser user)
        {
            if (HasProcessAuthorityby(user))
            {
                return CanbeUnBlockedCondition();
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region "处理"

        protected virtual bool CanbeProcessedCondition()
        {
            return this.IsTobeProcessed();
        }
        public virtual bool CanbeProcessedby(IUser user)
        {
            if (HasProcessAuthorityby(user))
            {
                return CanbeProcessedCondition();
            }
            else
            {
                return false;
            }
        }
        #endregion
        public virtual bool CanBeGeneratedPOby(IUser user)
        {
            if (this.IsTobeGenerated() || this.IsTobeProcessed())
            {
                return true;
            }
            return false;
        }
    }
}
