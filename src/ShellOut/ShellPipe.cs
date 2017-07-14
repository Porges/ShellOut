using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using static ShellOut.Utils;

namespace ShellOut
{
    public sealed class ShellPipe : Shell
    {
        private readonly Shell _left;
        private readonly Shell _right;

        public ShellPipe(Shell left, Shell right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override async Task ExecuteWithStreams(Stream input, Stream output, Stream error)
        {
            using (var serverPipe = new AnonymousPipeServerStream(PipeDirection.Out))
            using (var clientPipe = new AnonymousPipeClientStream(PipeDirection.In, serverPipe.ClientSafePipeHandle))
            {
                await Task.WhenAll(
                    RunLeft(input, serverPipe, error),
                    RunRight(clientPipe, output, error));
            }
        }
        
        private async Task RunLeft(Stream input, AnonymousPipeServerStream output, Stream error)
        {
            using (output)
            {
                await _left.ExecuteWithStreams(input, output, error);
                output.WaitForPipeDrain();
            }
        }

        private async Task RunRight(Stream input, Stream output, Stream error)
        {
            using (input)
            {
                await _right.ExecuteWithStreams(input, output, error);
            }
        }

        public override string ToString() => Invariant($"{_left} | {_right}");
    }
}