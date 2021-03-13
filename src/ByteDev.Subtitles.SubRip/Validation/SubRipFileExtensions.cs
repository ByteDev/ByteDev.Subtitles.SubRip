using System;
using ByteDev.Collections;

namespace ByteDev.Subtitles.SubRip.Validation
{
    /// <summary>
    /// Extension methods for <see cref="T:ByteDev.Subtitles.SubRip.SubRipFile" />.
    /// </summary>
    public static class SubRipFileExtensions
    {
        /// <summary>
        /// Checks whether all the entries order IDs are in sequential
        /// order from one onwards.
        /// </summary>
        /// <param name="source">SubRip file to check.</param>
        /// <returns>True if entries are in sequence; otherwise false.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
        public static bool IsOrderIdSequenceValid(this SubRipFile source)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));

            var expectedOrderId = SubRipFile.FirstOrderId;

            foreach (var subRipEntry in source.Entries)
            {
                if (subRipEntry.OrderId != expectedOrderId)
                    return false;

                expectedOrderId++;
            }

            return true;
        }

        /// <summary>
        /// Determine if any entry's duration overlaps with it's neighbor.
        /// </summary>
        /// <param name="source">SubRip file to check.</param>
        /// <returns>True if an entry's duration does overlap with it's neighbor.</returns>
        public static bool AnyEntryOverlap(this SubRipFile source)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));

            for (var i = 0; i < source.Entries.Count; i++)
            {
                var entry = source.Entries[i];

                var nextEntry = source.Entries.GetNext(i);

                if (nextEntry != null)
                {
                    if (entry.Duration.End >= nextEntry.Duration.Start)
                        return true;
                }
            }

            return false;
        }
    }
}