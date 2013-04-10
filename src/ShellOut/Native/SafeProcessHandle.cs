using System;
using System.Runtime.ConstrainedExecution;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace ShellOut.Native
{
    internal class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeProcessHandle()
            : base(true)
        { }

        [SecurityCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(DangerousGetHandle());
        }

        public override bool IsInvalid
        {
            get { return DangerousGetHandle() == IntPtr.Zero; }
        }

        public void Init(IntPtr value)
        {
            SetHandle(value);
        }
    }
}