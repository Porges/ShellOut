using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using ShellOut.Native;
using SafeProcessHandle = ShellOut.Native.SafeProcessHandle;

namespace ShellOut
{
    public class ShellProcess : Shell
    {
        private readonly string _executable;
        private readonly string[] _args;

        public ShellProcess(string executable, params object[] args)
        {
            if (executable == null)
            {
                throw new ArgumentNullException(nameof(executable));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            _executable = executable;
            _args = args.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)).ToArray();
        }

        public async override Task ExecuteWithPipes(SafeFileHandle input, SafeFileHandle output, SafeFileHandle error)
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

            SetInheritable(input);
            SetInheritable(output);
            SetInheritable(error);

            var startupInfo = StartupInfo.Create();
            startupInfo.Flags = StartFlags.UseStdHandles;
            startupInfo.StdInput = input;
            startupInfo.StdOutput = output;
            startupInfo.StdError = error;

            using (var processInfo = NativeMethods.CreateProcessChecked(BuildCommandLine(), ref startupInfo))
            {
                input.Dispose();
                output.Dispose();
                error.Dispose();

                using (var processHandle = new ProcessWaitHandle(processInfo.ProcessHandle))
                {
                    await processHandle;
                }
            }
        }
    
        private StringBuilder BuildCommandLine()
        {
            var sb = new StringBuilder();
            sb.Append('"');
            sb.Append(_executable);
            sb.Append("\" ");

            foreach (var arg in _args)
            {
                sb.Append(' ');

                EscapeTo(sb, arg);
            }
            return sb;
        }

        private static readonly char[] BadChars = {'"', ' '};
        private static void EscapeTo(StringBuilder builder, string arg)
        {
            // TODO: this is probably not completely correct
            if (arg.IndexOfAny(BadChars) > 0)
            {
                builder.Append('\"');
                builder.Append(arg.Replace("\"", "\\\""));
                builder.Append('\"');
            }
            else
            {
                builder.Append(arg);
            }
        }

        private static void SetInheritable(SafeFileHandle handle) => 
            NativeMethods.SetHandleInformationChecked(handle, HandleFlags.Inherit, HandleFlags.Inherit);

        private class ProcessWaitHandle : WaitHandle
        {
            public ProcessWaitHandle(SafeProcessHandle handle)
            {
                SafeWaitHandle = new SafeWaitHandle(handle.DangerousGetHandle(), false);
            }
        }

        public override string ToString() => BuildCommandLine().ToString();
    }
}