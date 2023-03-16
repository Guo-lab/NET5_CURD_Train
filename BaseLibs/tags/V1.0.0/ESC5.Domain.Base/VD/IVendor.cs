namespace ESC5.Domain.Base.VD
{
    public interface IVendor
    {
        int Id { get; }
        string E2bizCode { get; set; }
        string Code { get; set; }
        string Name { get; set; }
        bool InEvaluation();
        bool Certificated();
    }
}
