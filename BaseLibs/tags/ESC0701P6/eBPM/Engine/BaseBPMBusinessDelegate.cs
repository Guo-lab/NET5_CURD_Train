using ProjectBase.Domain.Transaction;

namespace eBPM.Engine
{
    public class BaseBPMBusinessDelegate : IBPMBusinessDelegate
    {
        public ITransactionHelper TransactionHelper { get; set; }
        public IContextCreator ContextCreator { get; set; }
    }
}
