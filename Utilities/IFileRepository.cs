namespace Utilities
{
    public interface IFileRepository
    {
        void CreateDirectoryWithPermissions(string directory);
        string GetFileName(string zipPath);
        bool ExistDirectory(string extractDirectory);
        void DeleteDirectory(string extractDirectory, bool v);
        List<string> EnumerateFilesFromAllDirectories(string path, string v);
        void ExtractToDirectory(string zipPath, string extractDirectory);
        string[] ReadAllLines(string pathToFile);
        bool Exists(string path);
        void Delete(string path);
        void CreateFile(string path);
        void DeleteZipFile(string pathForZipFile);
        bool DirectoryExists(string path);
        void DeleteDirectory(string path);
        string[] GetFiles(string path, string searchPattern);
        string[] GetFiles(string path);
        public string[] GetDirectories(string path);
        public void CreateEmptyZip(string folderToZip,string pathToZipFile);
    }
}