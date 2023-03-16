using System;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System.Collections.Generic;

namespace eBPM.DomainModel.WF
{
    public class Entity : BaseDomainObject
    {
        public static readonly SortStruc<Entity>[] DefaultSort = new SortStruc<Entity>[]
                                                                    {
                                                                        new SortStruc<Entity>{
                                                                            OrderByExpression=o => o.Id,
                                                                            OrderByDirection = OrderByDirectionEnum.Asc
                                                                        }
                                                               };
        #region "persistent properties"

        public virtual string EntityName { get; set; }
        public virtual int? LatestVersion { get; set; }

        #endregion


        #region "methods"

        #endregion

        #region "Enums"

        #endregion
    }

}

