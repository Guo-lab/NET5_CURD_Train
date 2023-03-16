using eBPM.DomainModel.WorkFlowObject;
using eBPM.Role;
using ESC5.Domain.Base.VD;
using ESC5.ValueObject;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESC5.Domain.Base.VD
{
    [MappingIgnore]
    public abstract class VdrStatusChangeBase<ProcessT, ProcessorT> : BaseWorkFlowObject<ProcessT, ProcessorT> where ProcessT : BaseProcess, new()
                                                                                                where ProcessorT : BaseCurrentProcessor
    {


        #region "访问权限"
 
        public virtual bool HasAccessAuthorityby(IUser user)
        {
            return user.Id == this.RequestOwner.Id || user.ReportTo(this.RequestOwner.Supervisor);
        }

        #endregion

        #region "Cancel相关"
        /// <summary>
        /// 可取消的条件
        /// 符合以下3个条件之一可取消 1 被拒绝 2 草稿 3最后一次提交后尚未审批
        /// 子类可重载此方法，例如不支持取消功能时可直接返回false
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanbeCancelledCondition(IUser user)
        {
            if (NotYetChecked(user))
            {
                return true;
            }
            else if ( (this.IsRejected() || this.IsDraft()) &&  this.RequestOwner.Id == user.Id)
            {
                return true;
            }
            return false;


            //if ((this.IsRejected() && this.RequestOwner.Id == user.Id ) || this.IsDraft())
            //{
            //    return true;
            //}
            //return false;
        }

        /// <summary>
        /// 检查用户能否取消状态变更申请
        /// 实际项目中可在StatusChange类中重载此方法
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool CanbeCancelledby(IUser user)
        {
            if(this.RequestOwner.Id == user.Id)
            {
                return true;
            }
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeCancelledCondition(user);
            }
            return false;
        }

        protected abstract void SetCancelStatus();
        /// <summary>
        /// 取消状态变更申请
        /// </summary>
        /// <param name="comments"></param>
        public virtual void Cancel()
        {
            SetCancelStatus();
        }

        public virtual void Cancel(string comments, IUser user)
        {
            SetCancelStatus();
            //AddCancelProcess(user, comments);
        }

        #endregion


        #region"审批相关"
        //状态变更申请是否可被当前用户审批
        public virtual bool CanbeCheckedby(IUser processor)
        {
            return this.IsTobeApproved() && this.Processor.Count(x => x.Processor.Id == processor.Id) > 0;
        }
        #endregion

        #region "修改相关"
        /// <summary>
        /// 可修改的条件
        /// 符合以下3个条件之一可修改 1 被拒绝 2 已批准（可以升版本） 3最后一次提交后尚未审批
        /// 子类可重载此方法，例如不支持取消功能时可直接返回false
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanbeUpdatedCondition(IUser user)
        {
            if (this.IsDraft()) { return false; }
            if (this.IsCancelled()) { return false; }
            if (this.IsRejected() && this.RequestOwner.Id == user.Id ) { return true; }
            if (this.IsApproved()) { return false; }
            if (this.PendingProcessor != null) { return false; }
            if (NotYetChecked(user)) { return true; }
            return false;
        }
        /// <summary>
        /// 状态变更申请是否可被当前用户修改
        /// 实际项目中可在VdrStatusChange类中重载此方法
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public virtual bool CanbeUpdatedby(IUser processor)
        {
            if (HasAccessAuthorityby(processor))
            {
                return CanbeUpdatedCondition(processor);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region "提交/生成相关"

        /// <summary>
        /// 可提交的条件
        ///提交状态不限制
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanbeSubmittedCondition()
        {
            //return  this.IsDraft();
            if(this.IsApproved() || this.IsRejected() || this.IsCancelled()) { return false; }
            if (this.PendingProcessor != null) { return false; }
            return true;
        }

        /// <summary>
        /// 是否可提交
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool CanbeSubmittedby(IUser user)
        {
            if (HasAccessAuthorityby(user))
            {
                return CanbeSubmittedCondition();
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region "撤销相关"
        public virtual bool CanbeUndoneby(int userId)
        {
            if (this.IsApproved() || this.IsRejected() )
            {
                return false;
            }
            ProcessT? lastProcess = this.Process.LastOrDefault(x => x.Processedby.Id == userId);
            if (lastProcess == null)
            {
                return false;
            }
            return this.Process.Count(x => x.Id > lastProcess.Id) == 0;
        }
        #endregion


        #region "状态翻译"
        public abstract bool IsDraft();
        public abstract bool IsApproved();
        public abstract bool IsCancelled();
        public abstract bool IsRejected();
        public abstract bool IsTobeApproved();

        /// <summary>
        /// 最近一次提交后，下一个审批人尚未处理，此时可以重新提交或者取消
        /// </summary>
        /// <returns></returns>
        protected abstract bool NotYetChecked(IUser user);
        #endregion

        #region "添加Process"
        protected abstract void AddCancelProcess(IUser user, string comments);
        #endregion

    }
}
