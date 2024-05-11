using System.IO;
using System.IO.Compression;

namespace Utilities
{
    public class ArchiveEntry
    {
        private ZipArchiveEntry _zipArchiveEntry;

        public ArchiveEntry(ZipArchiveEntry zipArchiveEntry)
        {
            _zipArchiveEntry = zipArchiveEntry;
        }

        public string Name => _zipArchiveEntry.Name;
        public string FullName => _zipArchiveEntry.FullName;

        public Stream Open()
        {
            return _zipArchiveEntry.Open();
        }
    }
}
