using System.Text.RegularExpressions;
using ByteDev.Strings;

namespace ByteDev.Subtitles.SubRip
{
    internal static class StringExtensions
    {
        public static string RemoveBold(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            return source.Replace("<b>", string.Empty)
                .Replace("</b>", string.Empty)
                .Replace("{b}", string.Empty)
                .Replace("{/b}", string.Empty);
        }

        public static string RemoveItalic(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            return source.Replace("<i>", string.Empty)
                .Replace("</i>", string.Empty)
                .Replace("{i}", string.Empty)
                .Replace("{/i}", string.Empty);
        }

        public static string RemoveUnderline(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            return source.Replace("<u>", string.Empty)
                .Replace("</u>", string.Empty)
                .Replace("{u}", string.Empty)
                .Replace("{/u}", string.Empty);
        }

        public static string RemoveFont(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            var text = Regex.Replace(source, @"<font[^>]*>", string.Empty);

            return Regex.Replace(text, @"</.*font.*>", string.Empty);
        }

        public static bool ContainsReturnChar(this string source)
        {
            if (source == null)
                return false;

            return source.Contains("\r") || source.Contains("\n");
        }

        public static bool EndsWithSentenceEndChar(this string source)
        {
            var endChar = source.Right(1);

            return endChar == "." || endChar == "," || endChar == ";";
        }
    }
}