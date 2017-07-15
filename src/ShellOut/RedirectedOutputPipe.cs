using System;
using System.IO;
using System.Threading.Tasks;
using static ShellOut.Utils;

namespace ShellOut
{
    public class RedirectedOutputPipe : Shell
    {
        private readonly IStreamProvider _handle;
        private readonly Shell _inner;

        private RedirectedOutputPipe(Shell inner, IStreamProvider provider)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _handle = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public RedirectedOutputPipe(Shell inner, string filename)
            : this(inner, new FileOpeningHandleProvider(filename, FileMode.Create))
        { }

        public RedirectedOutputPipe(Shell inner, Stream stream)
            : this(inner, new StreamWritingHandleProvider(stream))
        { }

        public override async Task ExecuteWithStreams(Stream input, Stream _, Stream error)
        {
            using (var output = _handle.CreateStream())
            {
                await _inner.ExecuteWithStreams(input, output, error);
            }
        }

        public override string ToString() => Invariant($"{_inner} > {_handle}");
    }
}