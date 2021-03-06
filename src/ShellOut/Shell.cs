﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace ShellOut
{
    public abstract class Shell
    {
        public static ShellProcess Run(string executable, params object[] args) => new ShellProcess(executable, args);

        public static ShellPipe operator |(Shell left, Shell right) => new ShellPipe(left, right);

        public static Shell operator >(Shell left, string right) => new RedirectedOutputPipe(left, right);

        public static Shell operator <(Shell left, string right) => new RedirectedInputPipe(left, right);

        public static Shell operator >(Shell left, Stream right) => new RedirectedOutputPipe(left, right);

        public static Shell operator <(Shell left, Stream right) => new RedirectedInputPipe(left, right);

        public Task Execute() => ExecuteWithStreams(null, null, null);

        public abstract Task ExecuteWithStreams(Stream input, Stream output, Stream error);
    }
}