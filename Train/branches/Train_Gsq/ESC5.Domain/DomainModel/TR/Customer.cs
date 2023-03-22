using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;



namespace ESC5.Domain.DomainModel.TR
{
    public class Customer : BaseDomainObject /// 继承BaseDomainObject的类--DO类
    { 
        public static readonly SortStruc<Customer>[] DefaultSort = new SortStruc<Customer>[]
        {
            new SortStruc<Customer>{
                OrderByExpression=o => o.Name_,
                OrderByDirection = OrderByDirectionEnum.Asc
            }
        };

        // Id PK defined
        #region "persistent properties" -- 字段对应属性定义
        [DomainSignature] //// 标记业务唯一性属性（一个或多个）。每个DO类都必须标记
        public virtual string Email { get; set; }

        [RefText("Email+Name_")]
        public virtual string Name_ { get; set; }

        public virtual int Gender { get; set; }
        public virtual DateTime? RegisterDate { get; set; }
        public virtual decimal Spending { get; set; }


        // --------- Enum Fail ? ----------
        public virtual int Vip { get; set; }
        //public virtual RankEnum Vip { get; set; }
        public virtual bool Active { get; set; }

        #endregion

        #region "enums"
        /// <summary>
        /// 用于数据库字段的枚举类型一般应在DO类内部定义（内部类），便于使用。
        /// 用于多个表时也可作为独立的枚举类来定义。
        /// </summary>
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
