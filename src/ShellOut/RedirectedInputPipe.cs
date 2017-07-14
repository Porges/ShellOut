using System;
using System.IO;
using System.Threading.Tasks;
using static ShellOut.Utils;

namespace ShellOut
{
    public class RedirectedInputPipe : Shell
    {
        private readonly IStreamProvider _handle;
        private readonly Shell _inner;

        private RedirectedInputPipe(Shell inner, IStreamProvider provider)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _handle = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public RedirectedInputPipe(Shell inner, string filename)
            : this(inner, new FileOpeningHandleProvider(filename, FileMode.Open))
        { }
        
        public RedirectedInputPipe(Shell inner, Stream stream)
            : this(inner, new StreamReadingHandleProvider(stream))
        { }
        
        public override async Task ExecuteWithStreams(Stream _, Stream output, Stream error)
        {
            using (var input = _handle.CreateStream())
            {
                await _inner.ExecuteWithStreams(input, output, error);
            }
        }
        
        public override string ToString() => Invariant($"{_inner} < {_handle}");
    }
}