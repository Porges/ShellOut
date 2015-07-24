using System;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace ShellOut.Native
{
    [SuppressUnmanagedCodeSecurity]
    internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeProcessHandle()
            : base(true)
        { }

        protected override bool ReleaseHandle() =>
            NativeMethods.CloseHandle(DangerousGetHandle());

        public override bool IsInvalid => DangerousGetHandle() == IntPtr.Zero;

        public void Init(IntPtr value) => SetHandle(value);
    }
}