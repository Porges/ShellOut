using System;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using ShellOut.Native;
using static ShellOut.Utils;

namespace ShellOut
{
    public class ShellPipe : Shell
    {
        private readonly Shell _left;
        private readonly Shell _right;

        public ShellPipe(Shell left, Shell right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            _left = left;
            _right = right;
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

            SafeFileHandle readPipe;
            SafeFileHandle writePipe;

            NativeMethods.CreatePipeChecked(out readPipe, out writePipe);

            using (var error2 = new SafeFileHandle(error.DangerousGetHandle(), false)) // TODO: a better way to do this?
            {
                await Task.WhenAll(
                    RunLeft(input, writePipe, error),
                    RunRight(readPipe, output, error2));
            }
        }

        private async Task RunLeft(SafeFileHandle input, SafeFileHandle output, SafeFileHandle error)
        {
            using (output)
            {
                await _left.ExecuteWithPipes(input, output, error);
            }
        }

        private async Task RunRight(SafeFileHandle input, SafeFileHandle output, SafeFileHandle error)
        {
            using (input)
            {
                await _right.ExecuteWithPipes(input, output, error);
            }
        }

        public override string ToString() => Invariant($"{_left} | {_right}");
    }
}