using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class FileRepository : IFileRepository
    {
        public string GetFileName(string zipPath)
        {
            return Path.GetFileNameWithoutExtension(zipPath);
        }
        public void CreateDirectoryWithPermissions(string directory)
        {

            if (ExistDirectory(directory))
            {
                return;
            }
            Directory.CreateDirectory(directory);
            SetFullControlPermissionsToEveryone(directory);

        }
        public void CreateWithPermitionsIfNotExist(string directory)
        {
            try
            {
                if (ExistDirectory(directory))
                {
                    DeleteDirectory(directory, true);
                }
                Directory.CreateDirectory(directory);
                SetFullControlPermissionsToEveryone(directory);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(e.Message);
            }

        }
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public void CreateFile(string path)
        {
            File.Create(path).Close();
        }
        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public void DeleteDirectory(string path)
        {
            if (!ExistDirectory(path))
            {
                return;
            }
            Directory.Delete(path, true); // true = deletes all content in directory as well
        }

        public void DeleteZipFile(string pathForZipFile)
        {
            if (Exists(pathForZipFile))
            {
                Delete(pathForZipFile);
            }
        }
        public void CreateEmptyZip(string folderToZip, string pathToZipFile)
        {
            string tempFolder = Path.Combine(folderToZip, "log");
            Directory.CreateDirectory(tempFolder);
            
            if (File.Exists(pathToZipFile))
                File.Delete(pathToZipFile);

            ZipFile.CreateFromDirectory(tempFolder, pathToZipFile);
            Directory.Delete(tempFolder);
        }
        private static void SetFullControlPermissionsToEveryone(string path)
        {
            const FileSystemRights rights = FileSystemRights.FullControl;
            var allUsers = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var accessRule = new FileSystemAccessRule(
            allUsers,
            rights,
            InheritanceFlags.None,
            PropagationFlags.NoPropagateInherit,
            AccessControlType.Allow);
            var info = new DirectoryInfo(path);
            var security = info.GetAccessControl(AccessControlSections.Access);
            bool result;
            security.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);

            if (!result)
            {
                throw new InvalidOperationException("Failed to give full-control permission to all users for path " + path);
            }
            var inheritedAccessRule = new FileSystemAccessRule(
            allUsers,
            rights,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
            PropagationFlags.InheritOnly,
            AccessControlType.Allow);
            bool inheritedResult;
            security.ModifyAccessRule(AccessControlModification.Add, inheritedAccessRule, out inheritedResult);
            if (!inheritedResult)
            {
                throw new InvalidOperationException("Failed to give full-control permission inheritance to all users for " + path);
            }
            info.SetAccessControl(security);
        }
        public DirectoryInfo CreateDirectory(string directoryPath)
        {
            return Directory.CreateDirectory(directoryPath);
        }

        public void DeleteDirectory(string directoryPath, bool recursive)
        {
            Directory.Delete(directoryPath, recursive);
        }

        public bool ExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        public void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
        {
            ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
        }

        public List<string> EnumerateFilesFromAllDirectories(string path, string v)
        {
            return Directory.EnumerateFiles(path, v,SearchOption.AllDirectories).ToList();
        }
      
    }
}
