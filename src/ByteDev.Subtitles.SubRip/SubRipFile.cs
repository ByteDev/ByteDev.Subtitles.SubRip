using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ByteDev.Collections;
using ByteDev.Strings;
using ByteDev.Subtitles.SubRip.Validation;

namespace ByteDev.Subtitles.SubRip
{
    /// <summary>
    /// Represents an entire SubRip (.srt) file.
    /// </summary>
    public class SubRipFile
    {
        internal const int FirstOrderId = 1;

        private const string NewLine = "\r\n";
        private const string EntryDelimiter = NewLine + NewLine;

        /// <summary>
        /// File name.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// File's entries.
        /// </summary>
        public List<SubRipEntry> Entries { get; } = new List<SubRipEntry>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipFile" /> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="fileContents">Contents of the SubRip file.</param>
        public SubRipFile(string fileName, string fileContents)
        {
            FileName = fileName;
            AddEntries(fileContents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipFile" /> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="entries">List of SubRip entries.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="entries" /> is null.</exception>
        public SubRipFile(string fileName, List<SubRipEntry> entries)
        {
            FileName = fileName;
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        /// <summary>
        /// Sorts of all the entries so they are sequential based on their order ID.
        /// </summary>
        public void Sort()
        {
            Entries.Sort();
        }

        /// <summary>
        /// Sequentially set each entry's OrderId.
        /// </summary>
        public void SetOrderIds()
        {
            for (var i = FirstOrderId; i <= Entries.Count; i++)
            {
                Entries[i - 1].OrderId = i;
            }
        }

        /// <summary>
        /// Set the duration of all entries. If a entry duration will overlap with the next entry and
        /// overlaps are not allowed then the new end time will be one millisecond less than the next start time.
        /// </summary>
        /// <param name="subRipTimeSpan">Time span to set all entries duration to.</param>
        /// <param name="doNotAllowOverlaps">Optional flag to indicate if end time should never overlap start time of next entry.</param>
        public void SetAbsoluteDuration(SubRipTimeSpan subRipTimeSpan, bool doNotAllowOverlaps = false)
        {
            for (var i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];

                if (doNotAllowOverlaps)
                {
                    var nextEntry = Entries.GetNext(i);

                    if (nextEntry == null)
                    {
                        // entry is last one
                        entry.Duration.End = entry.Duration.Start.Add(subRipTimeSpan);
                    }
                    else
                    {
                        var ts = entry.Duration.Start.Add(subRipTimeSpan);

                        if (ts > nextEntry.Duration.Start)
                            entry.Duration.End = nextEntry.Duration.Start.Subtract(new SubRipTimeSpan(0, 0, 0, 1));
                        else
                            entry.Duration.End = ts;
                    }
                }
                else
                {
                    entry.Duration.End = entry.Duration.Start.Add(subRipTimeSpan);
                }
            }
        }

        /// <summary>
        /// Sets the duration on any entries that are over the provided timespan.
        /// </summary>
        /// <param name="subRipTimeSpan">Time span to any entries duration to if over.</param>
        public void SetMaxDuration(SubRipTimeSpan subRipTimeSpan)
        {
            foreach (var entry in Entries)
            {
                if (entry.Duration.TotalActiveTime > subRipTimeSpan)
                {
                    entry.Duration.End = entry.Duration.Start.Add(subRipTimeSpan);
                }
            }
        }

        /// <summary>
        /// For each entry makes sure the text on each line is not longer than specified.
        /// Any entry text that already contains a carriage return will not be altered.
        /// </summary>
        /// <param name="maxLineLength">Max line length in number of characters.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="maxLineLength" /> must be greater than zero.</exception>
        public void SetTextLineMaxLength(int maxLineLength)
        {
            if (maxLineLength < 1)
                throw new ArgumentOutOfRangeException(nameof(maxLineLength), "Max line length must be greater than zero.");

            foreach (var entry in Entries)
            {
                if (entry.Text.Length <= maxLineLength)
                    continue;

                if (entry.Text.ContainsReturnChar())
                    continue;

                var endText = entry.Text.Right(maxLineLength);

                var sb = new StringBuilder();

                int start = 0;
                int charsAdded = 0;

                while (charsAdded < entry.Text.Length)
                {
                    var line = entry.Text.SafeSubstring(start, maxLineLength);

                    if (line.Length == maxLineLength && 
                        line != endText && 
                        !line.EndsWithSentenceEndChar())
                    {
                        int i = line.Length - 1;

                        // Walk back to first space
                        while (line[i] != ' ')
                        {
                            line = entry.Text.SafeSubstring(start, i);
                            i--;
                        }

                        start = start + i + 1;
                    }
                    else
                    {
                        start += line.Length;
                    }
                    
                    sb.AppendIfNotEmpty(NewLine);

                    charsAdded += line.Length;
                    sb.Append(line.TrimEnd());
                }
                
                entry.Text = sb.ToString();
            }
        }

        /// <summary>
        /// Removes any text formatting from each entry. Formatting removed includes
        /// bold, italic, underline and font tags.
        /// </summary>
        public void RemoveTextFormatting()
        {
            foreach (var entry in Entries)
            {
                entry.Text = entry.Text
                    .RemoveBold()
                    .RemoveItalic()
                    .RemoveUnderline()
                    .RemoveFont();
            }
        }

        /// <summary>
        /// Removes any return characters from each entry's text.
        /// </summary>
        public void RemoveTextReturnChars()
        {
            foreach (var entry in Entries)
            {
                entry.Text = entry.Text
                    .Replace("\r\n", " ")
                    .Replace("\n", " ")
                    .TrimEnd();
            }
        }

        /// <summary>
        /// Removes all entries with the specified Order ID.
        /// </summary>
        /// <param name="orderId">Order ID.</param>
        public void RemoveEntry(int orderId)
        {
            if (!orderId.IsValidOrderId())
                throw new ArgumentOutOfRangeException(nameof(orderId), orderId, "Order ID must be one or more.");

            Entries.RemoveAll(e => e.OrderId == orderId);
        }
        
        /// <summary>
        /// Returns a string representation of the SubRip file's contents.
        /// </summary>
        /// <returns>String representation of the SubRip file's contents.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var entry in Entries)
            {
                sb.Append(entry);
                sb.Append(EntryDelimiter);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Loads a SubRip (.srt) file from disk.
        /// </summary>
        /// <param name="filePath">Full path to the SubRip file.</param>
        /// <param name="validateOnLoad">Optional. True perform extra validation onload of the file; false do not perform extra file validation.</param>
        /// <returns><see cref="T:ByteDev.Subtitles.SubRip.SubRipFile" /> created by the file.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="filePath" /> is null.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">SubRip file does not exist.</exception>
        /// <exception cref="T:ByteDev.Subtitles.SubRip.SubRipException">Extra file related validation failed.</exception>
        public static SubRipFile Load(string filePath, bool validateOnLoad = false)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            var fileContents = File.ReadAllText(filePath);
            var fileName = Path.GetFileName(filePath);

            var subRipFile = new SubRipFile(fileName, fileContents);

            if (validateOnLoad)
                new SubRipFileValidator().Validate(subRipFile);

            return subRipFile;
        }

        private void AddEntries(string fileContents)
        {
            var textEntries = GetTextEntries(fileContents);

            foreach (var textEntry in textEntries)
            {
                Entries.Add(new SubRipEntry(textEntry));
            }
        }

        private static IEnumerable<string> GetTextEntries(string fileContents)
        {
            fileContents = fileContents?.Trim();

            if (string.IsNullOrEmpty(fileContents))
                return Enumerable.Empty<string>();

            return Regex.Split(fileContents, EntryDelimiter);
        }
    }
}
