using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eBPM.Engine;

namespace eBPM.Role
{
    public interface IProcessorFinder
    {
        IList<IUser> FindProcesssor(BPMContext context);
    }

    public interface IProcessorFinderFactory
    {
        IProcessorFinder CreateProcessorFinder(string fullName);
    }
}
