using ProjectBase.Domain;
namespace eBPM.Engine
{
    public interface IContextCreator:IBusinessDelegate
    {
        T CreateMyContext<T,OrderT>(OrderT order, string action, string comments, FinishStepModeEnum finishMode=FinishStepModeEnum.OneUser) where T :BPMContext,new();
    }
}
