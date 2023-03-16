using eBPM.Role;

namespace ESC5.Domain.Base.VD
{
    public interface IVdrStatusChange
    {
        int Id { get; }
        //int Status { get; set; }
               
        bool CanbeCancelledby(IUser user);
        bool CanbeUpdatedby(IUser user);
        bool CanbeSubmittedby(IUser user);
        bool CanbeCheckedby(IUser user);

        bool IsDraft();
        bool IsApproved();
        bool IsCancelled();

        void Cancel();

       
    }
}
