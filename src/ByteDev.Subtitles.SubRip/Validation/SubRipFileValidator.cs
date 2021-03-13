namespace ByteDev.Subtitles.SubRip.Validation
{
    /// <summary>
    /// Represents a validator of a SubRip file.
    /// </summary>
    public class SubRipFileValidator
    {
        /// <summary>
        /// Perform extra validation on the provided SubRip file.
        /// </summary>
        /// <param name="subRipFile">SubRip file to perform the validation on.</param>
        public void Validate(SubRipFile subRipFile)
        {
            if (!subRipFile.IsOrderIdSequenceValid())
                throw new SubRipException("SubRip file contains entries with Order ID in the wrong order/position. Order IDs should be sequential from one onwards.");

            if (subRipFile.AnyEntryOverlap())
                throw new SubRipException("SubRip file contains entries whose durations overlap with their neighbor.");
        }
    }
}