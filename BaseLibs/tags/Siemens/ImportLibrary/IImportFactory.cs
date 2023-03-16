
namespace ImportLibrary
{
    public interface IImportFactory
    {
        IImport CreateImport(string fullName);        
    }
}
