using System;

namespace ShellOut.Native
{
    internal sealed class ProcessInfo : IDisposable
    {
        private readonly SafeProcessHandle _processHandle;
        private readonly SafeProcessHandle _threadHandle;
        private readonly int _processId;
        private readonly int _threadId;

        public ProcessInfo(SafeProcessHandle processHandle, SafeProcessHandle threadHandle, int processId, int threadId)
        {
            _processHandle = processHandle;
            _threadHandle = threadHandle;
            _processId = processId;
            _threadId = threadId;
        }

        public SafeProcessHandle ProcessHandle => _processHandle;

        public int ProcessId => _processId;

        public int ThreadId => _threadId;

        public void Dispose()
        {
            using (_processHandle)
            using (_threadHandle)
            { }
        }
    }
}