using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESC5.ValueObject;

namespace ESC5.Exchange
{
    public class DeliveryCommitmentDTO
    {
        public int Id { get; set; }

        public int POId { get; set; }
        public int POItemId { get; set; }
        public int POVersion { get; set; }
        public int CommitmentVersion { get; set; }

        public decimal RequiredQuantity { get; set; }
        public UnitVO RequiredUnit { get; set; }
        public DateTime RequiredDeliveryDate { get; set; }
        public decimal CommittedQuantity { get; set; }
        public UnitVO CommittedUnit { get; set; }
        public DateTime CommittedDeliveryDate { get; set; }
        public DateTime SubmitTime { get; set; }
    }

    
}
