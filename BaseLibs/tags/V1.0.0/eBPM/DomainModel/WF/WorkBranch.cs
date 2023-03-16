using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;

namespace eBPM.DomainModel.WF
{
    public class WorkBranch : BaseDomainObject
    {
        public static readonly SortStruc<WorkBranch>[] DefaultSort = new SortStruc<WorkBranch>[]
                                                                    {
                                                                        new SortStruc<WorkBranch>{
                                                                            OrderByExpression=o => o.Id,
                                                                            OrderByDirection = OrderByDirectionEnum.Asc
                                                                        }
                                                               };
        #region "persistent properties"

		public virtual WorkStep WorkStep { get; set; }
        public virtual string Action { get; set; }
		public virtual string Expression { get; set; }
		public virtual WorkStep NextStep { get; set; }
        public virtual string MappedProcessType { get; set; }


        #endregion


        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

}

