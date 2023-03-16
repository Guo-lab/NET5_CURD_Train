using eBPM.DomainModel.WorkFlowObject;
using eBPM.Role;
using ProjectBase.Domain;
using System;
using System.Linq;
namespace ESC5.Domain.Base.PO
{
    public abstract partial class POBase<ProcessT, ProcessorT, ItemT> : POBase<ProcessT, ProcessorT> where ProcessT : BaseProcess, new()
                                                                                                where ProcessorT : BaseCurrentProcessor
                                                                                                where ItemT : POItemBase

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

        #region "Cancel相关"
        /// <summary>
        /// 可取消的条件
        /// 符合以下3个条件之一可取消 1 被拒绝 2 最后一次提交后尚未审批
        /// 子类可重载此方法，例如不支持取消功能时可直接返回false
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanbeCancelledCondition()
        {
            if (NotYetChecked())
            {
                return true;
            }
            else if (this.IsRejected())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查用户能否取消订单
        /// 实际项目中可在PO类中重载此方法
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

        #region"Close/CloseItem相关"
        /// <summary>
        /// 关闭订单余量
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="processor"></param>
        public virtual void Close(string comments, IUser processor)
        {
            foreach (var item in this.Items)
            {
                if (!item.IsClosed())
                {
                    item.Close(processor);
         
                }
            }
            AddCloseProcess(processor, comments);
            CalcReceivingStatus();
            CalcSettlementStatus();
        }

        /// <summary>
        /// 关闭订单行余量
        /// </summary>
        /// <param name="poItemId"></param>
        /// <param name="comments"></param>
        /// <param name="processor"></param>
        public virtual void CloseItem(int poItemId, string comments, IUser processor)
        {
            ItemT? poItem = this.Items.FirstOrDefault(x => x.Id == poItemId);
            if (poItem==null || poItem.IsClosed()) { return; }
            poItem.Close(processor);

            if (this.Items.Count(x => !x.IsClosed()) == 0)
            {
                AddCloseProcess(processor, comments);
            }
            else
            {
                AddCloseItemProcess(processor, "(" + this.PONo + "-" + poItem.ItemNo + ")" + comments);
            }
            CalcReceivingStatus();
            CalcSettlementStatus();
        }

        protected virtual bool CanItembeClosedCondition(int itemId)
        {
            ItemT item = this.Items.First(x => x.Id == itemId);
            if (item.IsReceived() || item.IsClosed()) { return false; } //已收货完毕或者已经关闭的不能关闭

            if (this.Version == 1) //第一版只有在批准之后才需要关闭，否则可直接删除
            {
                if (this.IsApproved()) { return true; }
            }
            else //升版之后，如果当前可修改就可以直接关闭
            {
                if (this.CanbeUpdatedCondition()) { return true; }
            }

            return false;
        }
        public virtual bool CanbeClosedby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                foreach (ItemT item in this.Items)
                {
                    if (CanItembeClosedCondition(item.Id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool CanItembeClosedby(int itemId, IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanItembeClosedCondition(itemId);
            }
            return false;
        }
        #endregion

        #region "Delete Item相关"

        protected virtual bool CanItembeDeletedCondition(int itemId)
        {
            if (this.Version == 1)
            {
                return this.IsDraft() || this.IsRejected() || this.NotYetChecked();
            }
            else
            {
                return false; //审批过的订单不能删除行，只能关闭DR只能关闭不能删除
            }
        }
        public virtual bool CanItembeDeletedby(int itemId, IUser user)
        {
            if (HasAccessAuthorityby(user))
            {
                return CanItembeDeletedCondition(itemId);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region"审批相关"
        //订单是否可被当前用户审批
        public virtual bool CanbeCheckedby(IUser processor)
        {
            if (HasCheckedAuthorityby(processor))
            {
                return CanCheckedCondition();
            }
            else {
                return false;
            }
        }
        protected virtual bool CanCheckedCondition() {
            return this.IsTobeApproved();
        }
        protected virtual bool HasCheckedAuthorityby(IUser processor) { 
         return this.Processor.Count(x => x.Processor.Id == processor.Id) > 0;
        }
        #endregion

        #region "修改相关"
        /// <summary>
        /// 可修改的条件
        /// 符合以下4个条件之一可修改 1 被拒绝 2 已批准（可以升版本） 3最后一次提交后尚未审批 4 Draft
        /// 子类可重载此方法，例如不支持取消功能时可直接返回false
        /// 已收货订单(任何状态)不允许在修改订单和订单行
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanbeUpdatedCondition()
        {
            if (this.IsDraft()) { return true; }
            if (this.Items.Count == 0) { return false; }
            if (this.HasReceived()) {
                if (this.IsRejected()) { return false; }
                if (this.IsApproved()) { return false; }
                if (NotYetChecked()) { return false; }
                return false; 
            } else {
                if (this.IsRejected()) { return true; }
                if (this.IsApproved()) { return true; }
                if (NotYetChecked()) { return true; }
            }
            if (this.IsClosed()) { return false; }
            if (this.IsWriteOff()) { return false; }
            if (this.IsCancelled()) { return false; }
            if (this.IsTobeApproved()) { return false; }
            return false;
        }

        protected virtual bool CanbeSubmitCondition() {
            if (this.IsDraft()) { return true; }
            if (this.Items.Count == 0) { return false; }
            if (this.IsRejected()) { return true; }
            if (this.IsApproved()) { return true; }
            if (NotYetChecked()) { return true; }
            if (this.IsClosed()) { return false; }
            if (this.IsWriteOff()) { return false; }
            if (this.IsCancelled()) { return false; }
            return false;
        }
        /// <summary>
        /// 订单是否可被当前用户修改
        /// 实际项目中可在PO类中重载此方法
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public virtual bool CanbeUpdatedby(IUser processor)
        {
            if (HasUpdateAuthorityby(processor))
            {
                return CanbeUpdatedCondition();
            }
            else
            {
                return false;
            }
        }
        public virtual bool HasUpdateAuthorityby(IUser user) {
            return user.Id == this.RequestOwner.Id ;
        }
        /// <summary>
        /// 订单是否可被当前用户提交
        /// 实际项目中可在PO类中重载此方法
        /// </summary>
        /// <param name="processor"></param>
        /// <returns></returns>
        public virtual bool CanSubmitBy(IUser processor) {
            if (HasUpdateAuthorityby(processor))
            {
                return CanbeSubmitCondition();
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region "BO释放订单"
        public virtual bool CanbeReleasedby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //用户是否在检查范围
            {
                return CanbeReleasedCondition();
            }
            return false;
        }

        //缺省的可释放订单的条件
        //原材料订单可能可以在订单生效日之前释放订单，可以在子类中实现
        protected virtual bool CanbeReleasedCondition()
        {
            if (this.IsBlanketOrder() && this.IsApproved() && this.EffectiveFrom <= DateTime.Today &&
                this.EffectiveTo >= DateTime.Today && this.Items.Count(x => x.BalanceQuantity > 0) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region "添加新订单行"
        /// <summary>
        /// 添加新订单项的条件
        /// 缺省情况下订单生成后不能添加，可在子类中重载
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanAddNewCondition()
        {
            return false;
        }
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

        #region "撤销相关"
        public virtual bool CanbeUndoneby(int userId)
        {
            if (this.IsApproved())
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

        #region"订单金额计算"
        [MappingIgnore]
        public virtual decimal Amount
        {
            get
            {
                return this.Items.Sum(x => x.Amount);
            }
        }
        [MappingIgnore]
        public virtual decimal AmountVAT
        {
            get
            {
                return this.Items.Sum(x => x.AmountVAT);
            }
        }


        #endregion

        #region "状态翻译"
        public abstract bool IsDraft();
        public abstract bool IsClosed();
        public abstract bool IsApproved();
        public abstract bool IsCancelled();
        public abstract bool IsWriteOff();
        public abstract bool IsRejected();
        public abstract bool IsTobeApproved();

        /// <summary>
        /// 最近一次提交后，下一个审批人尚未处理，此时可以重新提交或者取消
        /// </summary>
        /// <returns></returns>
        protected abstract bool NotYetChecked();

        public abstract bool IsBlanketOrder();
        public abstract bool IsDeliveryRequest();
        protected abstract bool HasReceived();
        #endregion

        #region "添加Process"
        protected abstract void AddCloseProcess(IUser user, string comments);
        protected abstract void AddCloseItemProcess(IUser user, string comments);
        protected abstract void AddCancelProcess(IUser user, string comments);
        #endregion

        #region "计算收货状态"
        protected abstract void SetNotReceivedStatus();
        protected abstract void SetPartialReceivedStatus();
        protected abstract void SetReceivedorClosedStatus();
        public virtual void CalcReceivingStatus()
        {
            if (IsBlanketOrder()) { return; }
            if (this.Items.Count(x => x.OpenQuantity == x.Quantity) == this.Items.Count)
            {
                SetNotReceivedStatus();
                return;
            }
            if (this.Items.Count(x => x.OpenQuantity < x.Quantity && x.OpenQuantity > 0) > 0)
            {
                SetPartialReceivedStatus();
                return;
            }

            if (this.Items.Count(x => x.OpenQuantity <= 0) == this.Items.Count)
            {
                SetReceivedorClosedStatus();
            }
        }
        #endregion

        #region "计算结算状态"
        protected abstract void SetNotSettledStatus();
        protected abstract void SetSettledStatus();
        public virtual void CalcSettlementStatus()
        {
            if (IsBlanketOrder()) { return; }
            //收货未完成
            if (this.Items.Count(x => x.OpenQuantity > 0) > 0)
            {
                SetNotSettledStatus();
                return;
            }
            //收货已完成或已关闭
            if (this.NeedSettlement)
            {
                //收货数量不等于结算数量，说明有未结算的入库单
                if (this.Items.Count(x => x.ReceivedQuantity != x.SettledQuantity) > 0)
                {
                    SetNotSettledStatus();
                }
                else
                {
                    SetSettledStatus();
                }
            }
            else
            {
                SetSettledStatus();
                return;
            }
        }
        #endregion
    }

}

