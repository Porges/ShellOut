﻿using System;
using System.IO;
using Microsoft.Win32.SafeHandles;
using ShellOut.Native;

namespace ShellOut
{
    public class FileOpeningHandleProvider : IHandleProvider
    {
        private readonly string _filename;
        private readonly FileMode _mode;

        public FileOpeningHandleProvider(string filename, FileMode mode)
        {
            if (mode != FileMode.Create && mode != FileMode.Open)
            {
                throw new ArgumentException("mode must be Create or Open", nameof(mode));
            }

            _filename = filename ?? throw new ArgumentNullException(nameof(filename));
            _mode = mode;
        }

        public SafeFileHandle CreateHandle() => NativeMethods.CreateFileChecked(_filename, _mode);

        public override string ToString() => "\"" + _filename + "\"";
    }
}