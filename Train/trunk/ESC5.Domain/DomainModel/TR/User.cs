using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System;

namespace ESC5.Domain.DomainModel.TR
{
    public class User:BaseDomainObject
    {
        public static readonly SortStruc<User>[] DefaultSort = new SortStruc<User>[]
                                                                    {
                                                                        new SortStruc<User>{
                                                                            OrderByExpression=o => o.Name,
                                                                            OrderByDirection = OrderByDirectionEnum.Asc
                                                                        }
                                                               };
        #region "persistent properties"

        [DomainSignature]
        public virtual string Code { get; set; }
        [RefText("Code+Name")]
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual decimal Salary { get; set; }
        public virtual RankEnum Rank { get; set; }
        public virtual bool Active { get; set; }

        #endregion

        #region "enums"
        public enum RankEnum
        {
            Junior = 1,
            Middle = 2,
            Senior = 3,
            Super = 4
        }
        #endregion
    }
}
