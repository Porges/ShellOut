using System;
using System.IO;

namespace ShellOut
{
    internal class StreamWritingHandleProvider : IStreamProvider
    {
        private readonly Stream _stream;

        public StreamWritingHandleProvider(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException("Stream is not writable", nameof(stream));
            }

            _stream = stream;
        }

        public Stream CreateStream() => _stream;

        public override string ToString() => _stream.ToString();
    }
}