using System;
using NUnit.Framework;

namespace ByteDev.Subtitles.SubRip.UnitTests
{
    [TestFixture]
    public class SubRipEntryTests
    {
        private const string Text = "We will now make a short stop\r\n" +
                                    "in what is called a kibbutz";

        private SubRipDuration _duration;

        [SetUp]
        public void SetUp()
        {
            _duration = new SubRipDuration(new SubRipTimeSpan(0, 1, 20, 0), new SubRipTimeSpan(0, 1, 23, 500));
        }

        [TestFixture]
        public class Constructor_Parts : SubRipEntryTests
        {
            [Test]
            public void WhenOrderIdLessThanOne_ThenThrowException()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SubRipEntry(0, _duration, Text));
            }

            [Test]
            public void WhenDurationIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = new SubRipEntry(1, null, Text));
            }

            [Test]
            public void WhenTextIsNull_ThenSetTextToEmpty()
            {
                var sut = new SubRipEntry(1, _duration, null);

                Assert.That(sut.Text, Is.Empty);
            }

            [Test]
            public void WhenTextWrappedInWhiteSpace_ThenTrimText()
            {
                var sut = new SubRipEntry(1, _duration, " " + Text + "\r\n");

                Assert.That(sut.Text, Is.EqualTo(Text));
            }

            [Test]
            public void WhenValid_ThenSetProperties()
            {
                var sut = new SubRipEntry(1, _duration, Text);

                Assert.That(sut.OrderId, Is.EqualTo(1));

                Assert.That(sut.Duration.Start.Hours, Is.EqualTo(0));
                Assert.That(sut.Duration.Start.Minutes, Is.EqualTo(1));
                Assert.That(sut.Duration.Start.Seconds, Is.EqualTo(20));
                Assert.That(sut.Duration.Start.Milliseconds, Is.EqualTo(0));

                Assert.That(sut.Duration.End.Hours, Is.EqualTo(0));
                Assert.That(sut.Duration.End.Minutes, Is.EqualTo(1));
                Assert.That(sut.Duration.End.Seconds, Is.EqualTo(23));
                Assert.That(sut.Duration.End.Milliseconds, Is.EqualTo(500));

                Assert.That(sut.Text, Is.EqualTo(Text));
            }
        }

        [TestFixture]
        public class Constructor_String : SubRipEntryTests
        {
            [TestCase(null)]
            [TestCase("")]
            public void WhenEntryIsNullOrEmpty_ThenThrowException(string entry)
            {
                Assert.Throws<ArgumentException>(() => _ = new SubRipEntry(entry));
            }

            [Test]
            public void WhenIsWrappedInWhiteSpace_ThenSetProperties()
            {
                const string entry = " \r\n" +  
                                     "1\r\n" +  
                                     "00:01:20,000 --> 00:01:23,500\r\n" +
                                     Text + " \r\n";

                var sut = new SubRipEntry(entry);

                Assert.That(sut.OrderId, Is.EqualTo(1));
                Assert.That(sut.Duration.ToString(), Is.EqualTo("00:01:20,000 --> 00:01:23,500"));
                Assert.That(sut.Text, Is.EqualTo(Text));
            }

            [Test]
            public void WhenIsMissingOrderId_ThenThrowException()
            {
                const string entry = "00:01:20,000 --> 00:01:23,500\r\n" +
                                     Text;
                
                var ex = Assert.Throws<SubRipException>(() => _ = new SubRipEntry(entry));
                Assert.That(ex.Message, Is.EqualTo("Invalid SubRip entry string representation."));
            }
            
            [Test]
            public void WhenIsMissingDuration_ThenThrowException()
            {
                const string entry = "1\r\n" + 
                                     Text;
                
                var ex = Assert.Throws<SubRipException>(() => _ = new SubRipEntry(entry));
                Assert.That(ex.Message, Is.EqualTo("Invalid SubRip entry string representation."));
            }

            [Test]
            public void WhenIsMissingText_ThenSetTextToEmpty()
            {
                const string entry = "1\r\n" +
                                     "00:01:20,000 --> 00:01:23,500";

                var sut = new SubRipEntry(entry);

                Assert.That(sut.OrderId, Is.EqualTo(1));
                Assert.That(sut.Duration.ToString(), Is.EqualTo("00:01:20,000 --> 00:01:23,500"));
                Assert.That(sut.Text, Is.Empty);
            }
        }

        [TestFixture]
        public class ToStringOverride : SubRipEntryTests
        {
            [Test]
            public void WhenCalled_ThenReturnsString()
            {
                const string expected = "1\r\n" +
                                        "00:01:20,000 --> 00:01:23,500\r\n" +
                                        Text;

                var sut = new SubRipEntry(1, _duration, Text);

                var result = sut.ToString();

                Assert.That(result, Is.EqualTo(expected));
            }
        }
    }
}
