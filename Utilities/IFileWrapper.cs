namespace Utilities
{
    public interface IFileWrapper
    {
        Task<string> CreateZipFileFromDirectory(string vin, string pathToLogs);
    }
}