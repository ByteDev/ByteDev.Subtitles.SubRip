using System;
using NUnit.Framework;

namespace ByteDev.Subtitles.SubRip.UnitTests
{
    [TestFixture]
    public class SubRipDurationTests
    {
        [TestFixture]
        public class Constructor_String : SubRipDurationTests
        {
            [Test]
            public void WhenValid_ThenSetProperties()
            {
                var sut = new SubRipDuration("00:01:20,000 --> 00:01:23,000");

                Assert.That(sut.Start.ToString(), Is.EqualTo("00:01:20,000"));
                Assert.That(sut.End.ToString(), Is.EqualTo("00:01:23,000"));
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase("00:01:20,000 --> 00:01:23,0001")]
            [TestCase("A0:01:20,000 --> 00:01:23,000")]
            [TestCase("00:01:20,000=====00:01:23,000")]
            public void WhenInvalid_ThenThrowException(string duration)
            {
                Assert.Throws<ArgumentException>(() => _ = new SubRipDuration(duration));
            }

            [Test]
            public void WhenStartTimeGreaterThanEnd_ThenThrowException()
            {
                Assert.Throws<ArgumentException>(() => _ = new SubRipDuration("00:01:20,001 --> 00:01:20,000"));
            }
        }

        [TestFixture]
        public class Constructor_SubRipTimeSpans : SubRipDurationTests
        {
            [Test]
            public void WhenValid_ThenSetProperties()
            {
                var start = new SubRipTimeSpan("00:01:20,000");
                var end = new SubRipTimeSpan("00:01:23,000");

                var sut = new SubRipDuration(start, end);

                Assert.That(sut.Start.ToString(), Is.EqualTo("00:01:20,000"));
                Assert.That(sut.End.ToString(), Is.EqualTo("00:01:23,000"));
                Assert.That(sut.TotalActiveTime.ToString(), Is.EqualTo("00:00:03,000"));
            }

            [Test]
            public void WhenStartTimeGreaterThanEnd_ThenThrowException()
            {
                var start = new SubRipTimeSpan("00:01:20,001");
                var end = new SubRipTimeSpan("00:01:20,000");

                Assert.Throws<ArgumentException>(() => _ = new SubRipDuration(start, end));
            }
        }

        [TestFixture]
        public class Duration : SubRipDurationTests
        {
            [Test]
            public void WhenStartLessThanEnd_ThenReturnTimeSpan()
            {
                var sut = new SubRipDuration("00:01:20,000 --> 00:01:23,000");

                Assert.That(sut.TotalActiveTime.ToString(), Is.EqualTo("00:00:03,000"));                
            }

            [Test]
            public void WhenStartAndEndAreEqual_ThenReturnTimeSpan()
            {
                var sut = new SubRipDuration("00:01:20,000 --> 00:01:20,000");

                Assert.That(sut.TotalActiveTime.TotalMilliseconds, Is.EqualTo(0));                
            }
        }
    }
}
