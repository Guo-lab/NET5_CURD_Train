using ESC5.Domain.Base.Exception;
using ProjectBase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ESC5.Domain.Base.IM
{
    [MappingIgnore]
    public abstract class PartBase : DomainObjectWithUniqueId,IPart
    {
        public virtual string PartNo { get; set; }
        public virtual string Description { get; set; }
        public virtual bool StockMaterial { get; set; }
        
        public virtual bool LotEnabled { get; set; } //�Ƿ��������Σ���Ϊtrue,�����ʱ�����������κ�
        public virtual bool SNEnabled { get; set; }//�Ƿ��������кţ���Ϊtrue,�����ʱ�����������к�
        public virtual bool ShelfLifeEnabled { get; set; } //�Ƿ����ñ����ڣ���Ϊtrue���ʱ�������뱣���ڽ�ֹ����
        public virtual bool IsActive { get; set; }
    }

    [MappingIgnore]
    public abstract class PartBase<UnitT> : PartBase where UnitT : PartUnitBase,new()
    {
        public virtual IList<UnitT> Units { get; set; }

        [MappingIgnore]
        public virtual UnitT PrimaryUnit
        {
            get
            {
                return this.Units.First(x => x.IsPrimary);
            }
        }

        public virtual void AddUnit(UnitT unit)
        {
            if (this.Units == null)
            {
                this.Units = new List<UnitT>();
                if (!unit.IsPrimary || unit.Rate != 1)
                {
                    throw new InvalidPrimaryUnitException();
                }
            }
            if (unit.IsPrimary && this.Units.Count(x => x.IsPrimary) > 0)
            {
                throw new DuplicatedPrimaryUnitException();
            }
            unit.Part = this;
            this.Units.Add(unit);
        }

        public virtual void AddPrimaryUnit(string unit)
        {
            this.AddUnit(new UnitT { Unit = unit, IsPrimary = true, Rate = 1 });
        }
    }
}

