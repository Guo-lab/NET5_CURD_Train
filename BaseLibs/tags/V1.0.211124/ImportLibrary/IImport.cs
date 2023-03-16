namespace ImportLibrary
{
    public interface IImport
    {
        IFileParser FileParser { get; set; }
        string Import();

        void SetImportParameters(params object[] importParameters);
    }
}
