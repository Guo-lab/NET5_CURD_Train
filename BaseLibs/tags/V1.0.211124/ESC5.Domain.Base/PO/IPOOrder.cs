using eBPM.Role;
using System;
namespace ESC5.Domain.Base.PO
{
    public interface IPOOrder
    {
        int Id { get; }
        string PONo { get; set; }
        int Status { get; set; }

        bool IsCloned { get; set; }
        DateTime? ApprovedTime { get; set; }
               
        bool CanbeCancelledby(IUser user);
        bool CanSubmitBy(IUser user);
        bool CanbeCheckedby(IUser user);

        bool CanbeReleasedby(IUser user);
        bool CanbeClosedby(IUser user);
        bool CanItembeClosedby(int itemId, IUser user);
        bool CanItembeDeletedby(int itemId, IUser user);

        bool CanAddNewItemby(IUser user);

        bool IsDraft();
        bool IsApproved();
        bool IsCancelled();
        bool IsWriteOff();

        void Close(string comments, IUser processor);
        void CloseItem(int poItemId, string comments, IUser processor);
        void Cancel();
    }
}
