using System;

namespace ByteDev.Subtitles.SubRip
{
    /// <summary>
    /// Represents a SubRip duration of time (start time to end time).
    /// </summary>
    public class SubRipDuration
    {
        private const string TimeDelimiter = " --> ";

        private static readonly int ValidStringLength = (SubRipTimeSpan.ValidStringLength * 2) + TimeDelimiter.Length;

        /// <summary>
        /// Start time.
        /// </summary>
        public SubRipTimeSpan Start { get; set; }

        /// <summary>
        /// End time.
        /// </summary>
        public SubRipTimeSpan End { get; set; }

        /// <summary>
        /// Total duration of time the subtitle is active.
        /// </summary>
        public SubRipTimeSpan TotalActiveTime => End.Subtract(Start);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipDuration" /> class.
        /// </summary>
        /// <param name="duration">SubRip duration in string format: "HH:MM:SS,MMM --> HH:MM:SS,MMM".</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="duration" /> should be in format: "HH:MM:SS,MMM --> HH:MM:SS,MMM".</exception>
        public SubRipDuration(string duration)
        {
            if (!IsValidDuration(duration))
                throw new ArgumentException($"Duration string: '{duration}' is in invalid format.");

            var start = new SubRipTimeSpan(duration.Substring(0, SubRipTimeSpan.ValidStringLength));
            var end = new SubRipTimeSpan(duration.Substring(SubRipTimeSpan.ValidStringLength + TimeDelimiter.Length));

            if (!IsValidStartAndEnd(start, end))
                throw new ArgumentException("Start time was after end time.");

            Start = start;
            End = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipDuration" /> class.
        /// </summary>
        /// <param name="start">Start time.</param>
        /// <param name="end">End time.</param>
        public SubRipDuration(SubRipTimeSpan start, SubRipTimeSpan end)
        {
            if (!IsValidStartAndEnd(start, end))
                throw new ArgumentException("Start time was after end time.");

            Start = start;
            End = end;
        }

        /// <summary>
        /// Returns a string representation of the SubRip duration.
        /// </summary>
        /// <returns>String representation of the SubRip duration.</returns>
        public override string ToString()
        {
            return Start + TimeDelimiter + End;
        }

        private static bool IsValidDuration(string duration)
        {
            if (duration?.Length != ValidStringLength)
                return false;

            if (duration.Substring(12, TimeDelimiter.Length) != TimeDelimiter)
                return false;

            return true;
        }

        private static bool IsValidStartAndEnd(SubRipTimeSpan start, SubRipTimeSpan end)
        {
            return start.TotalMilliseconds <= end.TotalMilliseconds;
        }
    }
}
