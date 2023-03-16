using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectBase.Domain;

namespace eBPM.Role
{
    
    public interface IUser
    {
        int Id { get; }

        string Code { get; set;}
        string Name { get; set; }
        string Email { get; set; }
        IUser Supervisor { get; set; }
        bool ReportTo(IUser user);
        bool CanAccess(string funcCode);
        decimal GetPOAuthority(int POType);
    }

}
