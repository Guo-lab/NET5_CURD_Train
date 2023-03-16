using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eBPM.DomainModel.WF;
using eBPM.Engine;
namespace eBPM.Param
{
    public interface IParamValueFinder
    {
        object FindParameterValue(BPMContext context);
    }

    public interface IParamValueFinderFactory
    {
        IParamValueFinder CreateParameterValueFinder(string fullName);
    }
}
