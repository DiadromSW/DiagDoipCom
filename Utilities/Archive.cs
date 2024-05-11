using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Utilities
{
    public class Archive : IDisposable
    {
        private ZipArchive _zipArchive;

        public Archive(Stream stream)
        {
            _zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
            }

        public void Dispose()
        {
            // This will also Dispose the underlying stream.
            _zipArchive.Dispose();
        }

        public IEnumerable<ArchiveEntry> Entries => _zipArchive.Entries.Select(x => new ArchiveEntry(x));

        public ArchiveEntry GetEntryByName(string name)
        {
            var entry = _zipArchive.Entries.SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return new ArchiveEntry(entry);
        }

        public IEnumerable<ArchiveEntry> GetEntriesByExtension(string extension)
        {
            var entries = _zipArchive.Entries.Where(x => Path.GetExtension(x.Name).Equals(extension, StringComparison.InvariantCultureIgnoreCase));
            return entries.Select(x => new ArchiveEntry(x));
        }
    }
}
