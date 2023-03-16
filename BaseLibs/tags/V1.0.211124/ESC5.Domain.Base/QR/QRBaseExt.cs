using eBPM.DomainModel.WorkFlowObject;
using eBPM.Role;
using System;
using System.Collections.Generic;

namespace ESC5.Domain.Base.QR
{
    public abstract partial class QRBase<ProcessT, ProcessorT, ItemT> : QRBase<ProcessT, ProcessorT> where ProcessT : BaseProcess, new()
                                                                                            where ProcessorT : BaseCurrentProcessor
                                                                                            where ItemT : QRItemBase

    {
        #region "访问权限"
        /// <summary>
        /// 检查用户是否对此订单有访问权限
        /// 缺省逻辑为订单的采购员或者与采购员同一个主管可以访问
        /// 实际项目可重载此方法，例如可以按PurchasingGroup是否相同来确定
        /// </summary>
        public virtual bool HasAccessAuthorityby(IUser user)
        {
            return user.Id == this.RequestOwner.Id || user.ReportTo(this.RequestOwner.Supervisor);
        }

        #endregion

        #region "状态翻译"
        public abstract bool IsTobePublish();
        public abstract bool IsCancelled();
        public abstract bool IsClosed();
        public abstract bool IsDued();
        public abstract bool IsInQuoting();
        public abstract bool IsTobeOpened();
        public abstract bool IsTobeClosed();
        
        #endregion


        #region "提交相关"
        /// <summary>
        /// 缺省逻辑为自己创建的询价单只能自己提交
        /// 实际项目可重载此方法，例如领导也可以提交
        /// </summary>
        public virtual bool CanbeSubmittedby(IUser user)
        {
            if (this.Id == 0)
            {
                return true;
            }
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeSubmitCondition();
            }
            return false;
        }
        protected virtual bool CanbeSubmitCondition()
        {
            //没有草稿状态，所有待发布可编辑提交  
            return IsTobePublish();
        }
        #endregion

        #region "发布相关"
        public virtual bool CanbePublishedby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbePublishCondition();
            }
            return false;
        }
        protected abstract bool CanbePublishCondition();
        #endregion

        #region "延期相关"
        public virtual bool CanbeExtenddby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeExtendCondition();
            }
            return false;
        }
        protected abstract bool CanbeExtendCondition();
        public abstract void Extend(DateTime extendTo);
        #endregion

        #region "报价相关"
        public virtual bool CanbeQuotedby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeQuotedCondition();
            }
            return false;
        }
        protected abstract bool CanbeQuotedCondition();
        public abstract void Quote(IEnumerable<QuotationDTO> quotationList, IUser buyer);

        #endregion

        #region "开标相关"
        public abstract void Open();
        #endregion

        #region "完成询价相关"
        public virtual bool CanbeClosedby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeClosedCondition();
            }
            return false;
        }
        protected abstract bool CanbeClosedCondition();
        public abstract void Closedby(IUser closedBy);
        #endregion

        #region "取消/删除相关"
        public virtual bool CanbeCancelledby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeCancelledCondition();
            }
            return false;
        }
        protected virtual bool CanbeCancelledCondition()
        {
            //默认仅逾期状态可以取消
            if (IsDued()) {
                return true;
            }
            return false;
        }
        public abstract void Cancel();
        #endregion

        #region "添加新订单行"
        /// <summary>
        /// 添加询价项的条件
        /// 缺省情况下订单生成后都可以，可在子类中重载
        /// </summary>
        /// <returns></returns>
        protected abstract bool CanAddNewCondition();

        /// <summary>
        /// 是否可添加新的订单项
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool CanAddNewItemby(IUser user)
        {
            if (HasAccessAuthorityby(user))
            {
                return CanAddNewCondition();
            }
            else
            {
                return false;
            }
        }
        #endregion




    }
}
