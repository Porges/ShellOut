using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using ShellOut.Native;

namespace ShellOut
{
    class StreamReadingHandleProvider : IHandleProvider
    {
        private readonly Stream _stream;

        public StreamReadingHandleProvider(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanRead) throw new ArgumentException("must be able to read from the stream", "stream");

            _stream = stream;
        }

        public SafeFileHandle CreateHandle()
        {
            SafeFileHandle writePipe;
            SafeFileHandle readPipe;
            NativeMethods.CreatePipeChecked(out readPipe, out writePipe);

            Task.Run(async () =>
            {
                using (var fs = new FileStream(writePipe, FileAccess.Write))
                {
                    await _stream.CopyToAsync(fs);
                }
            });

            return readPipe;
        }

        public override string ToString()
        {
            return _stream.ToString();
        }
    }
}