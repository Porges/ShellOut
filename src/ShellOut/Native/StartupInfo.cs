using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ShellOut.Native
{
    internal struct StartupInfo
    {
        public static StartupInfo Create()
        {
            return new StartupInfo
                       {
                           _cb = Marshal.SizeOf(typeof(StartupInfo)),
                           _stdError = new SafeFileHandle(IntPtr.Zero, false),
                           _stdOutput = new SafeFileHandle(IntPtr.Zero, false),
                           _stdInput = new SafeFileHandle(IntPtr.Zero, false)
                       };
        }

#pragma warning disable 169 // unmanaged code needs these
        private int _cb;
        [MarshalAs(UnmanagedType.LPTStr)]
        string _reserved;
        [MarshalAs(UnmanagedType.LPTStr)]
        string _desktop;
        [MarshalAs(UnmanagedType.LPTStr)]
        string _title;
        int _x;
        int _y;
        int _xSize;
        int _ySize;
        int _xCountChars;
        int _yCountChars;
        int _fillAttribute;
        private StartFlags _flags;
        short _showWindow;
        short _cbReserved2;
        IntPtr _reserved2;
#pragma warning restore 169

        private SafeFileHandle _stdInput;
        private SafeFileHandle _stdOutput;
        private SafeFileHandle _stdError;

        public StartFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        public SafeFileHandle StdInput
        {
            get { return _stdInput; }
            set { _stdInput = value; }
        }

        public SafeFileHandle StdOutput
        {
            get { return _stdOutput; }
            set { _stdOutput = value; }
        }

        public SafeFileHandle StdError
        {
            get { return _stdError; }
            set { _stdError = value; }
        }
    }
}