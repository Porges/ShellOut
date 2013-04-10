using System;

namespace ShellOut.Native
{
    [Flags]
    internal enum StartFlags
    {
        None = 0,
        UseStdHandles = 0x00000100,
    }
}