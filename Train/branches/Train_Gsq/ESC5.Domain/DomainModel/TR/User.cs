using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain.DomainModel;
using System;

namespace ESC5.Domain.DomainModel.TR
{
    public class User : BaseDomainObject /// 继承BaseDomainObject的类叫做DO类，对应有Id字段的数据表
    { /// 定义类型为SortStruc<>[] 的静态变量，用于定义默认排序规则
      /// 调用BD层查询方法时如果未指定排序则使用此处定义的缺省值  
      /// 如未定义此变量，缺省查询时不排序
        public static readonly SortStruc<User>[] DefaultSort = new SortStruc<User>[]
        {
            new SortStruc<User>{
                OrderByExpression=o => o.Name, /// 排序属性
                OrderByDirection = OrderByDirectionEnum.Asc
            }
        };

        // Ctrl k c, Ctrl k u, Shift Tab

        #region "persistent properties" -- 字段对应属性定义

        [DomainSignature] //// 标记业务唯一性属性（一个或多个）。每个DO类都必须标记
        public virtual string Code { get; set; }
        [RefText("Code+Name")]
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime? BirthDate { get; set; } //// 可空字段
        public virtual decimal Salary { get; set; }
        public virtual RankEnum Rank { get; set; }
        public virtual bool Active { get; set; }

        #endregion

        #region "enums" -- 枚举类型定义
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


        // -------- Add a new field --------
        private string mood;
        public virtual string Mood { get => mood; set => mood = value; }
    }
}
