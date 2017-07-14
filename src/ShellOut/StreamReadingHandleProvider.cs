using System;
using System.IO;

namespace ShellOut
{
    internal class StreamReadingHandleProvider : IStreamProvider
    {
        private readonly Stream _stream;

        public StreamReadingHandleProvider(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable", nameof(stream));
            }

            _stream = stream;
        }

        public Stream CreateStream() => _stream;

        public override string ToString() => _stream.ToString();
    }
}