using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System.Collections.Generic;
namespace eBPM.DomainModel.WF
{
    public class WorkFlow : BaseDomainObject
    {
        public static readonly SortStruc<WorkFlow>[] DefaultSort = new SortStruc<WorkFlow>[]
                                                                    {
                                                                        new SortStruc<WorkFlow>{
                                                                            OrderByExpression=o => o.Version,
                                                                            OrderByDirection = OrderByDirectionEnum.Asc
                                                                        }
                                                               };
        #region "persistent properties"

        [DomainSignature]
        public virtual Entity Entity { get; set; }
        [DomainSignature]
        public virtual int Version { get; set; }
		public virtual bool IsDraft { get; set; }
		public virtual string ChangedContent { get; set; }
		
		public virtual DateTime LastModified { get; set; }

        public virtual IList<WorkStep> Steps { get; set; }

        public virtual IList<Parameter> Parameters { get; set; }
        #endregion


        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

}

