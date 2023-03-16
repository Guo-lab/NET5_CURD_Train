
namespace ImportLibrary
{
    public interface IFileParserFactory
    {
        IFileParser CreateParser(string fullName);        
    }
}
