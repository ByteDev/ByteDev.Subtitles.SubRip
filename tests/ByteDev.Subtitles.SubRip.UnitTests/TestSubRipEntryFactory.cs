namespace ByteDev.Subtitles.SubRip.UnitTests
{
    public static class TestSubRipEntryFactory
    {
        public static SubRipEntry Create1()
        {
            return new SubRipEntry(1, new SubRipDuration("01:40:55,758 --> 01:40:58,426"), "Listen to me carefully, Carlito.");
        }

        public static SubRipEntry Create2()
        {
            return new SubRipEntry(2, new SubRipDuration("01:40:58,677 --> 01:41:02,013"), "Rudy says Pachanga is complaining\r\nabout being broke;");
        }

        public static SubRipEntry Create3()
        {
            return new SubRipEntry(3, new SubRipDuration("01:41:02,306 --> 01:41:04,640"), "that he doesn't have a single dollar.");
        }

        public static SubRipEntry Create4()
        {
            return new SubRipEntry(4, new SubRipDuration("01:41:04,808 --> 01:41:08,144"), "He's also saying that you're a piece of shit,");
        }

        public static SubRipEntry Create5()
        {
            return new SubRipEntry(5, new SubRipDuration("01:41:08,771 --> 01:41:12,106"), "and that you don't have the balls\r\nto take care of the problem,");
        }
    }
}