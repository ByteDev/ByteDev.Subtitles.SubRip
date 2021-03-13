using System;

namespace ByteDev.Subtitles.SubRip
{
    /// <summary>
    /// Represents a SubRip subtitle entry.
    /// </summary>
    public class SubRipEntry : IComparable
    {
        private const string NewLine = "\r\n";

        private int _orderId;
        private SubRipDuration _duration;
        private string _text;

        /// <summary>
        /// Order ID.
        /// </summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="value" /> must be one or more..</exception>
        public int OrderId
        {
            get => _orderId;
            set 
            {
                if (!value.IsValidOrderId())
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Order ID must be one or more.");

                _orderId = value;
            }
        }

        /// <summary>
        /// When the subtitle should appear.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="value" /> is null.</exception>
        public SubRipDuration Duration
        {
            get => _duration;
            set => _duration = value ?? throw new ArgumentNullException(nameof(value), "Duration cannot be null.");
        }

        /// <summary>
        /// Subtitle text.
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value == null ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipEntry" /> class.
        /// </summary>
        /// <param name="orderid">Order ID.</param>
        /// <param name="duration">Duration the subtitle should appear.</param>
        /// <param name="text">Subtitle text.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="orderid" /> must be one or more.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="duration" /> is null.</exception>
        public SubRipEntry(int orderid, SubRipDuration duration, string text)
        {
            OrderId = orderid;
            Duration = duration;
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipEntry" /> class.
        /// </summary>
        /// <param name="entry">String representation of the entry.</param>
        /// <exception cref="T:System.ArgumentException"><paramref name="entry" /> is null or empty.</exception>
        /// <exception cref="T:ByteDev.Subtitles.SubRip.SubRipException">Invalid SubRip entry string representation.</exception>
        public SubRipEntry(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                throw new ArgumentException(nameof(entry));

            entry = entry.Trim();

            try
            {
                OrderId = int.Parse(StringHelper.ReadLine(entry, 0));
                Duration = new SubRipDuration(StringHelper.ReadLine(entry, 1));
                Text = StringHelper.ReadLineToEnd(entry, 2);
            }
            catch (Exception ex)
            {
                throw new SubRipException("Invalid SubRip entry string representation.", ex);
            }
        }

        /// <summary>
        /// Returns a string representation of the SubRip entry.
        /// </summary>
        /// <returns>String representation of the SubRip entry.</returns>
        public override string ToString()
        {
            return _orderId + NewLine +
                Duration + NewLine +
                Text;
        }

        public override int GetHashCode()
        {
            return (_orderId + Text).GetHashCode();
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var entry = (SubRipEntry)obj;

            return _orderId.CompareTo(entry._orderId);
        }

        #endregion
    }
}
