using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class FileWrapper : IFileWrapper
    {
        private IFileRepository _fileRepository;
        private const string _systemFileName = "SystemLog.log";
        private const string _vinFileName = "VinLog.log";
        private const string _systemArchiveFileName = "ArchivedSystemLog.log";
        private const string _vinArchiveFileName = "VinLog.1.log";
        public FileWrapper(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        /// <summary>
        /// Creates a zip file for a specific VIN with all logs related
        /// </summary>
        /// <param name="vin">VIN used to find VIN specific logs</param>
        /// <param name="basePathToLogs">Path to all logs</param>
        /// <returns>Path to the zipfile</returns>
        public async Task<string> CreateZipFileFromDirectory(string vin, string basePathToLogs)
        {
            string pathToVinDirectory = $"{basePathToLogs}/{vin}";
            string pathToZipFile = $"{basePathToLogs}/DiagComErrorLogs.zip";
            return await CreateZipFile(basePathToLogs, pathToVinDirectory, pathToZipFile);
        }


        /// <summary>
        /// Generates a zip file from selected folder
        /// </summary>
        /// <param name="folderToZip">Path to folder to be zipped</param>
        /// <param name="pathToZipFile">Path to created zip</param>
        /// <returns></returns>
        private async Task<string> CreateZipFile(string basePathToLogs, string pathToVinDirectory, string pathToZipFile)
        {
            //Create empty ZipArchive to write entries to
            _fileRepository.CreateEmptyZip(basePathToLogs, pathToZipFile);

            using (FileStream zipToOpen = new FileStream(pathToZipFile, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    //SystemLogs
                    using (var writer = new StreamWriter(archive.CreateEntry(_systemFileName).Open()))
                    {
                        await CopyFileContentToStreamWriter(Path.Combine(basePathToLogs, _systemArchiveFileName), writer);
                        await CopyFileContentToStreamWriter(Path.Combine(basePathToLogs, _systemFileName), writer);
                     
                    }
                    //VinLogs
                    if (_fileRepository.DirectoryExists(pathToVinDirectory))
                    {
                        using (var writer = new StreamWriter(archive.CreateEntry(_vinFileName).Open()))
                        {
                            await CopyFileContentToStreamWriter(Path.Combine(pathToVinDirectory, _vinArchiveFileName), writer);
                            await CopyFileContentToStreamWriter(Path.Combine(pathToVinDirectory, _vinFileName), writer);

                        }
                    }
                }
            }

            return pathToZipFile;
        }
        private async Task CopyFileContentToStreamWriter(string fileName, StreamWriter writer)
        {
            if (File.Exists(fileName))
            {
                string logLine;
                using (var fileStream=new FileStream(fileName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
                using (var streamReader = new StreamReader(fileStream))
                    while ((logLine = streamReader.ReadLine()) != null)
                        await writer.WriteLineAsync(logLine);
            }
        }
    }
}
