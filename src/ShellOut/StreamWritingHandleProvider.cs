using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using ShellOut.Native;

namespace ShellOut
{
    class StreamWritingHandleProvider : IHandleProvider
    {
        private readonly Stream _stream;

        public StreamWritingHandleProvider(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanWrite) throw new ArgumentException("must be able to write to the stream", "stream");

            _stream = stream;
        }

        public SafeFileHandle CreateHandle()
        {
            SafeFileHandle writePipe;
            SafeFileHandle readPipe;
            NativeMethods.CreatePipeChecked(out readPipe, out writePipe);

            Task.Run(async () =>
            {
                using (var fs = new FileStream(readPipe, FileAccess.Read))
                {
                    await fs.CopyToAsync(_stream);
                }             
            });

            return writePipe;
        }

        public override string ToString()
        {
            return _stream.ToString();
        }
    }
}