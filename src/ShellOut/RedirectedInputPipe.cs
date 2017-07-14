using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using static ShellOut.Utils;

namespace ShellOut
{
    public class RedirectedInputPipe : Shell
    {
        private readonly IHandleProvider _handle;
        private readonly Shell _inner;

        public RedirectedInputPipe(Shell inner, IHandleProvider provider)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _handle = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public RedirectedInputPipe(Shell inner, string filename)
            : this(inner, GetFilenameProvider(filename))
        { }
        
        public RedirectedInputPipe(Shell inner, Stream stream)
            : this(inner, GetStreamHandleProvider(stream))
        { }

        private static IHandleProvider GetFilenameProvider(string filename)
            => new FileOpeningHandleProvider(filename, FileMode.Open);

        private static IHandleProvider GetStreamHandleProvider(Stream stream)
        {
            // special case FileStreams, as an optimization:
            if (stream is FileStream fs)
            {
                return new SimpleHandleProvider(new SafeFileHandle(fs.SafeFileHandle.DangerousGetHandle(), false));
            }

            return new StreamReadingHandleProvider(stream);
        }

        public override async Task ExecuteWithPipes(SafeFileHandle input, SafeFileHandle output, SafeFileHandle error)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            using (var inputHandle = _handle.CreateHandle())
            {
                await _inner.ExecuteWithPipes(inputHandle, output, error);
            }
        }
        
        public override string ToString() => Invariant($"{_inner} < {_handle}");
    }
}