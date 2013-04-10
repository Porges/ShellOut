using System.Globalization;
using Microsoft.Win32.SafeHandles;

namespace ShellOut
{
    class SimpleHandleProvider : IHandleProvider
    {
        private readonly SafeFileHandle _handle;

        public SimpleHandleProvider(SafeFileHandle handle)
        {
            _handle = handle;
        }

        public SafeFileHandle CreateHandle()
        {
            return _handle;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Handle: <{0}>", _handle.DangerousGetHandle());
        }
    }
}