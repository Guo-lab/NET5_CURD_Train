using Newtonsoft.Json;
namespace ESC5.ValueObject
{
    public class Attachment : SharpArch.Domain.DomainModel.ValueObject
    {
        public virtual string OriginalFileName { get; protected set; }
        public virtual string SavedFileName { get; protected set; }

        public Attachment()
        {

        }
        public Attachment(string originalFileName, string savedFileName)
        {
            this.OriginalFileName = originalFileName;
            this.SavedFileName = savedFileName;
        }
    }
}
