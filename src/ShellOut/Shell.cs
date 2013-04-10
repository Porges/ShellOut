using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using ShellOut.Native;

namespace ShellOut
{
    public abstract class Shell
    {
        public static ShellProcess Run(string executable, params object[] args)
        {
            return new ShellProcess(executable, args);
        }

        public static ShellPipe operator |(Shell left, Shell right)
        {
            return new ShellPipe(left, right);
        }

        public static Shell operator >(Shell left, string right)
        {
            return new RedirectedOutputPipe(left, right);
        }
        
        public static Shell operator <(Shell left, string right)
        {
            return new RedirectedInputPipe(left, right);
        }

        public static Shell operator >(Shell left, Stream right)
        {
            return new RedirectedOutputPipe(left, right);
        }

        public static Shell operator <(Shell left, Stream right)
        {
            return new RedirectedInputPipe(left, right);
        }

        public async Task Execute()
        {
            using (var stdIn = NativeMethods.GetStdHandleChecked(StdHandle.Input))
            using (var stdOut = NativeMethods.GetStdHandleChecked(StdHandle.Output))
            using (var stdErr = NativeMethods.GetStdHandleChecked(StdHandle.Error))
            {
                await ExecuteWithPipes(stdIn, stdOut, stdErr);
            }
        }

        public abstract Task ExecuteWithPipes(SafeFileHandle input, SafeFileHandle output, SafeFileHandle error);

    }
}