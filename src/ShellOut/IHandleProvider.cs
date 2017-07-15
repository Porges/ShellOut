using System.IO;

namespace ShellOut
{
    /// <summary>
    /// Provides a stream for <see cref="RedirectedInputPipe"/> or <see cref="RedirectedOutputPipe"/> to use.
    /// </summary>
    public interface IStreamProvider
    {
        /// <summary>
        /// Get a new stream to use. This should be safe to dispose.
        /// </summary>
        Stream CreateStream();
    }
}