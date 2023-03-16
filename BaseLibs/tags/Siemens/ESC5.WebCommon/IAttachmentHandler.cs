using ESC5.ValueObject;

namespace ESC5.WebCommon
{
    public interface IAttachmentHandler
    {
        void MoveToFormalFolder(AttachmentVM attachment, string saveAs = "");
        void MoveToFormalFolder(AttachmentVM attachment, bool withDateFolder);
    }
}
