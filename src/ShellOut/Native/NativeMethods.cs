using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace ShellOut.Native
{
    internal static class NativeMethods
    {
        private static IntPtr _stdIn;
        private static IntPtr _stdOut;
        private static IntPtr _stdErr;

        public static SafeFileHandle GetStdHandleChecked(StdHandle handle)
        {
            IntPtr cached;
            switch (handle)
            {
                case StdHandle.Input:
                    cached = _stdIn;
                    break;
                case StdHandle.Output:
                    cached = _stdOut;
                    break;
                case StdHandle.Error:
                    cached = _stdErr;
                    break;
                default:
                    cached = IntPtr.Zero;
                    break;
            }

            IntPtr result;
            if (cached != IntPtr.Zero)
            {
                result = cached;
            }
            else
            {
                result = GetStdHandle(handle);
                if (result == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                switch (handle)
                {
                    case StdHandle.Input:
                        _stdIn = result;
                        break;
                    case StdHandle.Output:
                        _stdOut = result;
                        break;
                    case StdHandle.Error:
                        _stdErr = result;
                        break;
                }
            }

            return new SafeFileHandle(result, false); // don't close this
        }

        // this returns IntPtr as you probably never want to close the standard handles, and
        // SafeHandle doesn't provide a facilty to return non-owning handles from PInvoke
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetStdHandle(StdHandle handle);

        public static void CreatePipeChecked(out SafeFileHandle readPipe, out SafeFileHandle writePipe)
        {
            if (!CreatePipe(out readPipe, out writePipe))
            {
                throw new Win32Exception();
            }
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreatePipe(
            out SafeFileHandle readPipe,
            out SafeFileHandle writePipe,
            IntPtr pipeAttributes = default(IntPtr),
            int nSize = 0);

        public static SafeFileHandle CreateFileChecked(string filename, FileMode mode)
        {
            if (mode != FileMode.Create && mode != FileMode.Open)
            {
                throw new ArgumentException("mode must be Create or Open", nameof(mode));
            }

            var result = CreateFile(filename,
                mode == FileMode.Create ? DesiredAccess.GenericWrite : DesiredAccess.GenericRead,
                0, IntPtr.Zero,
                mode == FileMode.Create ? CreationDisposition.CreateAlways : CreationDisposition.OpenExisting,
                0, IntPtr.Zero);

            if (result.IsInvalid)
            {
                result.Dispose();
                throw new Win32Exception();
            }

            return result;
        }

        enum CreationDisposition
        {
            CreateAlways = 2,
            OpenExisting = 3,
        }

        enum DesiredAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(
            [MarshalAs(UnmanagedType.LPWStr)] string fileName,
            DesiredAccess desiredAccess,
            int dwShareMode,
            IntPtr securityAttributes,
            CreationDisposition creationCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CreateProcess(
            IntPtr applicationName,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandLine,
            IntPtr processAttributes,
            IntPtr threadAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool inheritHandles,
            int creationFlags,
            IntPtr environment,
            IntPtr currentDirectory,
            ref StartupInfo startupInfo,
            out InteropProcessInfo processInformation
            );

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetHandleInformation(SafeHandle handle, HandleFlags mask, HandleFlags flags);

        public static void SetHandleInformationChecked(SafeHandle handle, HandleFlags mask, HandleFlags flags)
        {
            if (!SetHandleInformation(handle, mask, flags))
            {
                throw new Win32Exception();
            }
        }

        public static ProcessInfo CreateProcessChecked(
            StringBuilder commandLine,
            ref StartupInfo startupInfo)
        {
            var processHandle = new SafeProcessHandle();
            var threadHandle = new SafeProcessHandle();

            InteropProcessInfo processInformation;
            int errorCode;
            bool succeeded;
            
            // need to ensure the IntPtrs are converted to SafeWaitHandles so they don't leak in the case of an asynchronous exception
            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally
            {
                // can't do any allocation inside here, so be a bit careful
                succeeded = CreateProcess(IntPtr.Zero, commandLine, IntPtr.Zero, IntPtr.Zero, true,
                                          0, IntPtr.Zero, IntPtr.Zero, ref startupInfo, out processInformation);
                errorCode = Marshal.GetLastWin32Error();

                if (succeeded)
                {
                    processHandle.Init(processInformation.ProcessHandle);
                    threadHandle.Init(processInformation.ThreadHandle);
                }
            }

            if (!succeeded)
            {
                throw new Win32Exception(errorCode);
            }

            return new ProcessInfo(
                processHandle,
                threadHandle,
                processInformation.ProcessId,
                processInformation.ThreadId);
        }

        private struct InteropProcessInfo
        {
            #pragma warning disable 649
            private IntPtr _process;
            private IntPtr _thread;
            private int _processId;
            private int _threadId;
            #pragma warning restore 649

            public IntPtr ProcessHandle => _process;

            public IntPtr ThreadHandle => _thread;

            public int ProcessId => _processId;

            public int ThreadId => _threadId;
        }
    }
}