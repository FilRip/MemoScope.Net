using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WinFwk
{
    // thanks to:
    // http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
    public static class StringHelpers
    {
        private static readonly Dictionary<string, string> escapeMapping = new()
        {
            {"\"", @"\\\"},
            {"\\\\", @"\\"},
            {"\a", @"\a"},
            {"\b", @"\b"},
            {"\f", @"\f"},
            {"\n", @"\n"},
            {"\r", @"\r"},
            {"\t", @"\t"},
            {"\v", @"\v"},
            {"\0", @"\0"},
        };

        private static readonly Regex escapeRegex = new(string.Join("|", escapeMapping.Keys.ToArray()), RegexOptions.Compiled);

        public static string Escape(this string s)
        {
            return escapeRegex.Replace(s, EscapeMatchEval);
        }

        private static string EscapeMatchEval(Match m)
        {
            if (escapeMapping.ContainsKey(m.Value))
            {
                return escapeMapping[m.Value];
            }
            return escapeMapping[Regex.Escape(m.Value)];
        }
    }
}
