using System;
using System.IO;

namespace ShellOut
{
    public class FileOpeningHandleProvider : IStreamProvider
    {
        private readonly string _filename;
        private readonly FileMode _mode;

        public FileOpeningHandleProvider(string filename, FileMode mode)
        {
            if (mode != FileMode.Create && mode != FileMode.Open)
            {
                throw new ArgumentException("mode must be Create or Open", nameof(mode));
            }

            _filename = filename ?? throw new ArgumentNullException(nameof(filename));
            _mode = mode;
        }

        public Stream CreateStream() => new FileStream(_filename, _mode, Access, Share, 4096, true);

        private FileAccess Access => _mode == FileMode.Open ? FileAccess.Read : FileAccess.Write;
        private FileShare Share => _mode == FileMode.Open ? FileShare.Read : FileShare.None;

        public override string ToString() => "\"" + _filename + "\"";
    }
}