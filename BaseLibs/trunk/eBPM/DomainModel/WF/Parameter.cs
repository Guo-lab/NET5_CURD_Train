using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using eBPM.Engine;
namespace eBPM.DomainModel.WF
{
    public class Parameter : BaseDomainObject
    {
        public static readonly SortStruc<Parameter>[] DefaultSort = new SortStruc<Parameter>[]
                                                                    {
                                                                        new SortStruc<Parameter>{
                                                                            OrderByExpression=o => o.ParameterName,
                                                                            OrderByDirection = OrderByDirectionEnum.Asc
                                                                        }
                                                               };
        #region "persistent properties"

		[DomainSignature]
		public virtual WorkFlow WorkFlow { get; set; }
		[DomainSignature]
		public virtual string ParameterName { get; set; }
		public virtual ValueTypeEnum? ParameterType { get; set; }
		public virtual string MapToProperty { get; set; }
		public virtual string ParameterValueFinder { get; set; }
        public virtual Guid Keys { get; set; }

        #endregion


        #region "methods"
       
        #endregion

        #region "Enums"

        #endregion
    }

}

