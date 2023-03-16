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

        #region "����Ȩ��"
        /// <summary>
        /// ����û��Ƿ�Դ˶����з���Ȩ��
        /// ȱʡ�߼�Ϊ�����Ĳɹ�Ա������ɹ�Աͬһ�����ܿ��Է���
        /// ʵ����Ŀ�����ش˷�����������԰�PurchasingGroup�Ƿ���ͬ��ȷ��
        /// </summary>
        public virtual bool HasAccessAuthorityby(IUser user)
        {
            return user.Id == this.RequestOwner.Id || user.ReportTo(this.RequestOwner.Supervisor);
        }

        #endregion

        #region "Cancel���"
        /// <summary>
        /// ��ȡ��������
        /// ��������3������֮һ��ȡ�� 1 ���ܾ� 2 ���һ���ύ����δ����
        /// ��������ش˷��������粻֧��ȡ������ʱ��ֱ�ӷ���false
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
        /// ����û��ܷ�ȡ������
        /// ʵ����Ŀ�п���PO�������ش˷���
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool CanbeCancelledby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //�û��Ƿ��ڼ�鷶Χ
            {
                return CanbeCancelledCondition();
            }
            return false;
        }

        protected abstract void SetCancelStatus();

        /// <summary>
        /// ͨ�������� ȡ������
        /// </summary>
        /// <param name="comments"></param>
        public virtual void Cancel()
        {
            SetCancelStatus();
        }

        /// <summary>
        /// �ݸ�״ֱ̬��ȡ������Ҫ�������ȡ����Process
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="user"></param>
        public virtual void Cancel(string comments, IUser user)
        {
            SetCancelStatus();
            AddCancelProcess(user, comments);
        }

        #endregion

        #region"Close/CloseItem���"
        /// <summary>
        /// �رն�������
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
        /// �رն���������
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
            if (item.IsReceived() || item.IsClosed()) { return false; } //���ջ���ϻ����Ѿ��رյĲ��ܹر�

            if (this.Version == 1) //��һ��ֻ������׼֮�����Ҫ�رգ������ֱ��ɾ��
            {
                if (this.IsApproved()) { return true; }
            }
            else //����֮�������ǰ���޸ľͿ���ֱ�ӹر�
            {
                if (this.CanbeUpdatedCondition()) { return true; }
            }

            return false;
        }
        public virtual bool CanbeClosedby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //�û��Ƿ��ڼ�鷶Χ
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
            if (HasAccessAuthorityby(user)) //�û��Ƿ��ڼ�鷶Χ
            {
                return CanItembeClosedCondition(itemId);
            }
            return false;
        }
        #endregion

        #region "Delete Item���"

        protected virtual bool CanItembeDeletedCondition(int itemId)
        {
            if (this.Version == 1)
            {
                return this.IsDraft() || this.IsRejected() || this.NotYetChecked();
            }
            else
            {
                return false; //�������Ķ�������ɾ���У�ֻ�ܹر�DRֻ�ܹرղ���ɾ��
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

        #region"�������"
        //�����Ƿ�ɱ���ǰ�û�����
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

        #region "�޸����"
        /// <summary>
        /// ���޸ĵ�����
        /// ��������4������֮һ���޸� 1 ���ܾ� 2 ����׼���������汾�� 3���һ���ύ����δ���� 4 Draft
        /// ��������ش˷��������粻֧��ȡ������ʱ��ֱ�ӷ���false
        /// ���ջ�����(�κ�״̬)���������޸Ķ����Ͷ�����
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
        /// �����Ƿ�ɱ���ǰ�û��޸�
        /// ʵ����Ŀ�п���PO�������ش˷���
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
        /// �����Ƿ�ɱ���ǰ�û��ύ
        /// ʵ����Ŀ�п���PO�������ش˷���
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

        #region "BO�ͷŶ���"
        public virtual bool CanbeReleasedby(IUser user)
        {
            if (HasAccessAuthorityby(user)) //�û��Ƿ��ڼ�鷶Χ
            {
                return CanbeReleasedCondition();
            }
            return false;
        }

        //ȱʡ�Ŀ��ͷŶ���������
        //ԭ���϶������ܿ����ڶ�����Ч��֮ǰ�ͷŶ�����������������ʵ��
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

        #region "����¶�����"
        /// <summary>
        /// ����¶����������
        /// ȱʡ����¶������ɺ�����ӣ���������������
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanAddNewCondition()
        {
            return false;
        }
        /// <summary>
        /// �Ƿ������µĶ�����
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

        #region "�������"
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

        #region"����������"
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

        #region "״̬����"
        public abstract bool IsDraft();
        public abstract bool IsClosed();
        public abstract bool IsApproved();
        public abstract bool IsCancelled();
        public abstract bool IsWriteOff();
        public abstract bool IsRejected();
        public abstract bool IsTobeApproved();

        /// <summary>
        /// ���һ���ύ����һ����������δ������ʱ���������ύ����ȡ��
        /// </summary>
        /// <returns></returns>
        protected abstract bool NotYetChecked();

        public abstract bool IsBlanketOrder();
        public abstract bool IsDeliveryRequest();
        protected abstract bool HasReceived();
        #endregion

        #region "���Process"
        protected abstract void AddCloseProcess(IUser user, string comments);
        protected abstract void AddCloseItemProcess(IUser user, string comments);
        protected abstract void AddCancelProcess(IUser user, string comments);
        #endregion

        #region "�����ջ�״̬"
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

        #region "�������״̬"
        protected abstract void SetNotSettledStatus();
        protected abstract void SetSettledStatus();
        public virtual void CalcSettlementStatus()
        {
            if (IsBlanketOrder()) { return; }
            //�ջ�δ���
            if (this.Items.Count(x => x.OpenQuantity > 0) > 0)
            {
                SetNotSettledStatus();
                return;
            }
            //�ջ�����ɻ��ѹر�
            if (this.NeedSettlement)
            {
                //�ջ����������ڽ���������˵����δ�������ⵥ
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

