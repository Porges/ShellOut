﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace ShellOut
{
    public class RedirectedInputPipe : Shell
    {
        private readonly IHandleProvider _handle;
        private readonly Shell _inner;

        public RedirectedInputPipe(Shell inner, IHandleProvider provider)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            if (provider == null) throw new ArgumentNullException("provider");

            _inner = inner;
            _handle = provider;
        }

        public RedirectedInputPipe(Shell inner, string filename)
            : this(inner, GetFilenameProvider(filename))
        { }
        
        public RedirectedInputPipe(Shell inner, Stream stream)
            : this(inner, GetStreamHandleProvider(stream))
        { }

        private static IHandleProvider GetFilenameProvider(string filename)
        {
            return new FileOpeningHandleProvider(filename, FileMode.Open);
        }

        private static IHandleProvider GetStreamHandleProvider(Stream stream)
        {
            // special case FileStreams:
            var fs = stream as FileStream;
            if (fs != null)
            {
                return new SimpleHandleProvider(new SafeFileHandle(fs.SafeFileHandle.DangerousGetHandle(), false));
            }

            return new StreamReadingHandleProvider(stream);
        }

        public override async Task ExecuteWithPipes(SafeFileHandle input, SafeFileHandle output, SafeFileHandle error)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (output == null) throw new ArgumentNullException("output");
            if (error == null) throw new ArgumentNullException("error");

            using (var inputHandle = _handle.CreateHandle())
            {
                await _inner.ExecuteWithPipes(inputHandle, output, error);
            }
        }
        
        public override string ToString()
        {
            return string.Format("{0} < {1}", _inner, _handle);
        }
    }
}