using System;
using System.Text.RegularExpressions;

namespace ByteDev.Subtitles.SubRip
{
    /// <summary>
    /// Represents a span of time in SubRip format.
    /// </summary>
    public readonly struct SubRipTimeSpan : IEquatable<SubRipTimeSpan>
    {
        private readonly TimeSpan _timeSpan;

        internal const int ValidStringLength = 12;
        
        /// <summary>
        /// Hours.
        /// </summary>
        public int Hours => _timeSpan.Hours;

        /// <summary>
        /// Minutes.
        /// </summary>
        public int Minutes => _timeSpan.Minutes;

        /// <summary>
        /// Seconds.
        /// </summary>
        public int Seconds => _timeSpan.Seconds;

        /// <summary>
        /// Milliseconds.
        /// </summary>
        public int Milliseconds => _timeSpan.Milliseconds;

        /// <summary>
        /// Total hours.
        /// </summary>
        public double TotalHours => _timeSpan.TotalHours;

        /// <summary>
        /// Total minutes.
        /// </summary>
        public double TotalMinutes => _timeSpan.TotalMinutes;

        /// <summary>
        /// Total seconds.
        /// </summary>
        public double TotalSeconds => _timeSpan.TotalSeconds;

        /// <summary>
        /// Total milliseconds.
        /// </summary>
        public double TotalMilliseconds => _timeSpan.TotalMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipTimeSpan" /> struct.
        /// </summary>
        /// <param name="timeSpan">SubRip time span as a string in format: "HH:MM:SS,MMM"</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="timeSpan" />Time span must be in the format: HH:MM:SS,MMM.</exception>
        public SubRipTimeSpan(string timeSpan)
        {
            if (!IsValidFormat(timeSpan))
                throw new ArgumentException("Time span must be in the format: HH:MM:SS,MMM. Max value: 23:59:59:999.", nameof(timeSpan));
            
            _timeSpan = new TimeSpan(0, 
                GetHours(timeSpan),
                GetMinutes(timeSpan),
                GetSeconds(timeSpan),
                GetMilliseconds(timeSpan));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipTimeSpan" /> struct.
        /// </summary>
        /// <param name="hours">Hours.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="seconds">Seconds.</param>
        /// <param name="milliseconds">Milliseconds.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="hours" /> must be between 0 and 23.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minutes" /> must be between 0 and 59.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="seconds" /> must be between 0 and 59.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="milliseconds" /> must be between 0 and 999.</exception>
        public SubRipTimeSpan(int hours, int minutes, int seconds, int milliseconds)
        {
            if (!hours.IsValidHours())
                throw new ArgumentOutOfRangeException(nameof(hours), hours, "Hours must be between 0 and 23.");

            if (!minutes.IsValidMinutes())
                throw new ArgumentOutOfRangeException(nameof(minutes), minutes, "Minutes must be between 0 and 59.");

            if (!seconds.IsValidSeconds())
                throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Seconds must be between 0 and 59.");

            if (!milliseconds.IsValidMilliseconds())
                throw new ArgumentOutOfRangeException(nameof(milliseconds), milliseconds, "Milliseconds must be between 0 and 999.");

            _timeSpan = new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipTimeSpan" /> struct
        /// with the max possible length of time.
        /// </summary>
        /// <returns>New instance with all time properties set to max.</returns>
        public static SubRipTimeSpan MaxValue() => new SubRipTimeSpan(23, 59, 59, 999);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipTimeSpan" /> struct
        /// with the min possible length of time (i.e. zero).
        /// </summary>
        /// <returns>New instance with all time properties set to zero.</returns>
        public static SubRipTimeSpan MinValue() => new SubRipTimeSpan(0, 0, 0, 0);

        /// <summary>
        /// Return a new instance with the SubRip time span added.
        /// </summary>
        /// <param name="subRipTimeSpan">SubRip time span to add.</param>
        /// <returns>New instance with the SubRip time added.</returns>
        public SubRipTimeSpan Add(SubRipTimeSpan subRipTimeSpan)
        {
            var ts = ConvertSrtTimeSpanToTimeSpan(subRipTimeSpan);
            
            ts = _timeSpan.Add(ts);
            
            return ConvertTimeSpanToSrtTimeSpan(ts);
        }

        /// <summary>
        /// Return a new instance with the SubRip time span subtracted.
        /// </summary>
        /// <param name="subRipTimeSpan">SubRip time span to subtract.</param>
        /// <returns>New instance with the SubRip time subtracted.</returns>
        public SubRipTimeSpan Subtract(SubRipTimeSpan subRipTimeSpan)
        {
            var ts = ConvertSrtTimeSpanToTimeSpan(subRipTimeSpan);

            ts = _timeSpan.Subtract(ts);

            if (ts.TotalMilliseconds < 0)
                return new SubRipTimeSpan(0, 0, 0, 0);

            return ConvertTimeSpanToSrtTimeSpan(ts);
        }

        /// <summary>
        /// Returns a string representation of the SubRip time span.
        /// </summary>
        /// <returns>String representation of the SubRip time span.</returns>
        public override string ToString()
        {
            return _timeSpan.Hours.ToString("00") + ":" +
                _timeSpan.Minutes.ToString("00") + ":" +
                _timeSpan.Seconds.ToString("00") + "," +
                _timeSpan.Milliseconds.ToString("000");
        }

        #region Comparison methods

        public bool Equals(SubRipTimeSpan other)
        {
            return _timeSpan.Equals(other._timeSpan);
        }

        public override bool Equals(object obj)
        {
            return obj is SubRipTimeSpan other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _timeSpan.GetHashCode();
        }

        public static bool operator ==(SubRipTimeSpan left, SubRipTimeSpan right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SubRipTimeSpan left, SubRipTimeSpan right)
        {
            return !left.Equals(right);
        }

        public static bool operator >(SubRipTimeSpan left, SubRipTimeSpan right)
        {
            return left.TotalMilliseconds > right.TotalMilliseconds;
        }

        public static bool operator <(SubRipTimeSpan left, SubRipTimeSpan right)
        {
            return left.TotalMilliseconds < right.TotalMilliseconds;
        }

        public static bool operator >=(SubRipTimeSpan left, SubRipTimeSpan right)
        {
            return left.TotalMilliseconds >= right.TotalMilliseconds;
        }

        public static bool operator <=(SubRipTimeSpan left, SubRipTimeSpan right)
        {
            return left.TotalMilliseconds <= right.TotalMilliseconds;
        }

        #endregion

        internal static bool IsValidFormat(string timeSpan)
        {
            if (timeSpan == null)
                return false;

            // Valid format: "23:59:59,999"
            return Regex.IsMatch(timeSpan, "^[0-2][0-3]:[0-5][0-9]:[0-5][0-9],[0-9][0-9][0-9]$");
        }

        private static int GetHours(string ts)
        {
            return int.Parse(ts.Substring(0, 2));
        }

        private static int GetMinutes(string ts)
        {
            return int.Parse(ts.Substring(3, 2));
        }

        private static int GetSeconds(string ts)
        {
            return int.Parse(ts.Substring(6, 2));
        }

        private static int GetMilliseconds(string ts)
        {
            return int.Parse(ts.Substring(9, 3));
        }

        private static TimeSpan ConvertSrtTimeSpanToTimeSpan(SubRipTimeSpan subRipTimeSpan)
        {
            return new TimeSpan(0, subRipTimeSpan.Hours, subRipTimeSpan.Minutes, subRipTimeSpan.Seconds, subRipTimeSpan.Milliseconds);
        }

        private static SubRipTimeSpan ConvertTimeSpanToSrtTimeSpan(TimeSpan timeSpan)
        {
            return new SubRipTimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}
