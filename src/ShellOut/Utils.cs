using System;
using System.Globalization;

namespace ShellOut
{
    internal static class Utils
    {
        public static string Invariant(FormattableString s) =>
            string.Format(CultureInfo.InvariantCulture, s.Format, s.GetArguments());
    }
}
