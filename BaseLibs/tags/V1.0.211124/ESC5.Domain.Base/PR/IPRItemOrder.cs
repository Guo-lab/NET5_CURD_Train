using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBPM.Role;

namespace ESC5.Domain.Base.PR
{
    public interface IPRItemOrder
    {
        int Id { get; }
        string PRNo { get; set; }
        int ItemNo { get; set; }
        int Status { get; set; }

        bool CanbeAssignedby(IUser user);
        bool CanbeUpdatedby(IUser user);
        bool CanbeCancelledby(IUser user);
        bool CanbeReturnPRby(IUser user);
        bool CanBeGeneratedPOby(IUser user);

        bool IsDraft();
        bool POGenerated();

        bool IsTobeProcessed();
        bool CanbeProcessedby(IUser user);

        bool IsCancelled();
        void Cancel();

        bool IsBlocked();
        bool CanbeBlockedby(IUser user);
        bool CanbeUnBlockedby(IUser user);

        bool IsTobeAssigned();
        void AssignTo(IUser assignedTo);


    }
}
