using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace ShellOut
{
    public class ShellProcess : Shell
    {
        private readonly string _executable;
        private readonly string[] _args;

        public ShellProcess(string executable, params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            _executable = executable ?? throw new ArgumentNullException(nameof(executable));
            _args = args.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)).ToArray();
        }

        public async override Task ExecuteWithStreams(Stream input, Stream output, Stream error)
        {
            var psi =
                new ProcessStartInfo(_executable, BuildArguments())
                {
                    UseShellExecute = false,
                    RedirectStandardInput = input != null,
                    RedirectStandardError = error != null,
                    RedirectStandardOutput = output != null,                  
                };
            
            using (var process = Process.Start(psi))
            {
                var tasks = new List<Task>(3);

                if (input != null)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        using (var pi = process.StandardInput.BaseStream)
                        {
                            await input.CopyToAsync(pi);
                        }
                    }));
                }

                if (output != null)
                {
                    tasks.Add(process.StandardOutput.BaseStream.CopyToAsync(output));
                }

                if (error != null)
                {
                    tasks.Add(process.StandardError.BaseStream.CopyToAsync(error));
                }
                
                await Task.WhenAll(tasks);

                process.WaitForExit();
            }
        }
    
        private string BuildArguments()
        {
            var sb = new StringBuilder();
            foreach (var arg in _args)
            {
                sb.Append(' ');

                EscapeTo(sb, arg);
            }

            return sb.ToString();
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
        
        public override string ToString() => _executable + BuildArguments();
    }
}