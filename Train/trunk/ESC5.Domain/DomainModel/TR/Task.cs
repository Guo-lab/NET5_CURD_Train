using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;

namespace ESC5.Domain.DomainModel.TR
{
    public class Task: BaseDomainObject
    {
        public static readonly SortStruc<Task>[] DefaultSort = new SortStruc<Task>[]
                                                                    {
                                                                        new SortStruc<Task>{
                                                                            OrderByExpression=o => o.Name,
                                                                            OrderByDirection = OrderByDirectionEnum.Asc
                                                                        }
                                                               };
        #region "persistent properties"

        [DomainSignature]
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual User? User { get; set; }
        public virtual int MaxItemCount { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual decimal Score { get; set; }
        public virtual StatusEnum Status { get; set; }
        public virtual bool Active { get; set; }


       public virtual IList<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

        #endregion

        #region "enums"
        public enum StatusEnum
        {
            Staging = 1,
            OnGoing = 2,
            Closed = 3
        }
        #endregion
    }
}
