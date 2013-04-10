using System;

namespace ShellOut.Native
{
    /// <summary>
    /// See: http://msdn.microsoft.com/en-nz/library/windows/desktop/ms724935.aspx
    /// </summary>
    [Flags]
    internal enum HandleFlags
    {
        None = 0,
        Inherit = 1,
        ProtectFromClose = 2,
    }
}