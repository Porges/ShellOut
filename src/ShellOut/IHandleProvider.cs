using Microsoft.Win32.SafeHandles;

namespace ShellOut
{
    /// <summary>
    /// Provides a handle for <see cref="RedirectedInputPipe"/> or <see cref="RedirectedOutputPipe"/> to use.
    /// </summary>
    public interface IHandleProvider
    {
        /// <summary>
        /// Get a new handle to use. This should be safe to dispose.
        /// </summary>
        SafeFileHandle CreateHandle();
    }
}