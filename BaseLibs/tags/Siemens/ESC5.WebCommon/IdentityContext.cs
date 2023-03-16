using eBPM.Role;
using ProjectBase.AutoMapper;
using ProjectBase.Dto;
namespace ESC5.WebCommon
{
    public class IdentityMapperContext:BaseSelfMapperContext
    {
        public IUser CurrentUser { get; set; }
    }

    public class IdentityMergerContext : BaseSelfMergerContext
    {
        public IUser CurrentUser { get; set; }
    }
}
