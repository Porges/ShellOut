using Microsoft.Win32.SafeHandles;
using static ShellOut.Utils;

namespace ShellOut
{
    internal sealed class SimpleHandleProvider : IHandleProvider
    {
        private readonly SafeFileHandle _handle;

        public SimpleHandleProvider(SafeFileHandle handle)
        {
            _handle = handle;
        }

        public SafeFileHandle CreateHandle() => _handle;

        public override string ToString() => Invariant($"Handle: <{_handle.DangerousGetHandle()}>");
    }
}