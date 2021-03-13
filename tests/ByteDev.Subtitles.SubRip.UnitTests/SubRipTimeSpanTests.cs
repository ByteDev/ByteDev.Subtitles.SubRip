using System;
using NUnit.Framework;

namespace ByteDev.Subtitles.SubRip.UnitTests
{
    [TestFixture]
    public class SubRipTimeSpanTests
    {
        [TestFixture]
        public class Constructor_String : SubRipTimeSpanTests
        {
            [TestCase("00:00:00,000", 0, 0, 0, 0)]
            [TestCase("01:02:34,500", 1, 2, 34, 500)]
            [TestCase("23:59:59,999", 23, 59, 59, 999)]
            public void WhenIsValid_ThenSetProperties(string timeSpan, int expectedHours, int expectedMins, int expectedSecs, int expectedMs)
            {
                var sut = new SubRipTimeSpan(timeSpan);

                Assert.That(sut.Hours, Is.EqualTo(expectedHours));
                Assert.That(sut.Minutes, Is.EqualTo(expectedMins));
                Assert.That(sut.Seconds, Is.EqualTo(expectedSecs));
                Assert.That(sut.Milliseconds, Is.EqualTo(expectedMs));
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase(" 00:00:00,000")]
            [TestCase("00:00:00,000 ")]
            [TestCase("00:00:00,0000")]
            [TestCase("00:00:04,00")]
            [TestCase("00:00:00.000")]
            [TestCase("A0:00:00,000")]
            [TestCase("24:00:00,000")]
            [TestCase("00:60:00,000")]
            [TestCase("00:00:60,000")]
            public void WhenIsInvalidFormat_ThenThrowException(string value)
            {
                Assert.Throws<ArgumentException>(() => _ = new SubRipTimeSpan(value));
            }
        }

        [TestFixture]
        public class Constructor_Ints : SubRipTimeSpanTests
        {
            [Test]
            public void WhenArgsValid_ThenSetProperties()
            {
                var sut = new SubRipTimeSpan(1, 59, 59, 999);

                Assert.That(sut.Hours, Is.EqualTo(1));
                Assert.That(sut.Minutes, Is.EqualTo(59));
                Assert.That(sut.Seconds, Is.EqualTo(59));
                Assert.That(sut.Milliseconds, Is.EqualTo(999));
            }
        
            [TestCase(-1)]
            [TestCase(24)]
            public void WhenInvalidHours_ThenThrowException(int hours)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SubRipTimeSpan(hours, 1, 1, 1));
            }

            [TestCase(-1)]
            [TestCase(60)]
            public void WhenInvalidMinutes_ThenThrowException(int minutes)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SubRipTimeSpan(1, minutes, 1, 1));
            }

            [TestCase(-1)]
            [TestCase(60)]
            public void WhenInvalidSeconds_ThenThrowException(int seconds)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SubRipTimeSpan(1, 1, seconds, 1));
            }

            [TestCase(-1)]
            [TestCase(1000)]
            public void WhenInvalidMilliseconds_ThenThrowException(int ms)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SubRipTimeSpan(1, 1, 1, ms));
            }
        }

        [TestFixture]
        public class ToStringOverride : SubRipTimeSpanTests
        {
            [TestCase("00:00:00,000")]
            [TestCase("01:23:45,000")]
            [TestCase("01:23:45,678")]
            public void WhenMsZero_ThenReturnsString(string value)
            {
                var sut = new SubRipTimeSpan(value);

                var result = sut.ToString();

                Assert.That(result, Is.EqualTo(value));
            }
        }

        [TestFixture]
        public class Add : SubRipTimeSpanTests
        {
            [Test]
            public void WhenSubRipTimeSpanIsValid_ThenAdd()
            {
                var sut = new SubRipTimeSpan("01:23:45,600");

                var result = sut.Add(new SubRipTimeSpan(0, 10, 10, 0));

                Assert.That(result.Hours, Is.EqualTo(1));
                Assert.That(result.Minutes, Is.EqualTo(33));
                Assert.That(result.Seconds, Is.EqualTo(55));
                Assert.That(result.Milliseconds, Is.EqualTo(600));
            }
        }

        [TestFixture]
        public class Subtract : SubRipTimeSpanTests
        {
            [Test]
            public void WhenSubRipTimeSpanIsValid_ThenSubtract()
            {
                var sut = new SubRipTimeSpan("01:23:45,600");

                var result = sut.Subtract(new SubRipTimeSpan(0, 10, 10, 0));

                Assert.That(result.Hours, Is.EqualTo(1));
                Assert.That(result.Minutes, Is.EqualTo(13));
                Assert.That(result.Seconds, Is.EqualTo(35));
                Assert.That(result.Milliseconds, Is.EqualTo(600));
            }

            [Test]
            public void WhenAfterSubtractSameAmountTime_ThenReturnZero()
            {
                var sut = new SubRipTimeSpan("01:23:45,600");

                var result = sut.Subtract(new SubRipTimeSpan(1, 23, 45, 600));

                Assert.That(result.TotalMilliseconds, Is.EqualTo(0));
            }

            [Test]
            public void WhenAfterSubtractLessThanZeroTime_ThenReturnZero()
            {
                var sut = new SubRipTimeSpan("01:23:45,600");

                var result = sut.Subtract(new SubRipTimeSpan(1, 23, 46, 600));

                Assert.That(result.TotalMilliseconds, Is.EqualTo(0));

                Assert.That(result.Hours, Is.EqualTo(0));
                Assert.That(result.Minutes, Is.EqualTo(0));
                Assert.That(result.Seconds, Is.EqualTo(0));
                Assert.That(result.Milliseconds, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class MaxValue : SubRipTimeSpanTests
        {
            [Test]
            public void WhenCalled_ThenReturnsMax()
            {
                var sut = SubRipTimeSpan.MaxValue();

                Assert.That(sut.Hours, Is.EqualTo(23));
                Assert.That(sut.Minutes, Is.EqualTo(59));
                Assert.That(sut.Seconds, Is.EqualTo(59));
                Assert.That(sut.Milliseconds, Is.EqualTo(999));
            }
        }

        [TestFixture]
        public class MinValue : SubRipTimeSpanTests
        {
            [Test]
            public void WhenCalled_ThenReturnsMin()
            {
                var sut = SubRipTimeSpan.MinValue();

                Assert.That(sut.Hours, Is.EqualTo(0));
                Assert.That(sut.Minutes, Is.EqualTo(0));
                Assert.That(sut.Seconds, Is.EqualTo(0));
                Assert.That(sut.Milliseconds, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class EqualsOverride : SubRipTimeSpanTests
        {
            [Test]
            public void WhenTwoAreEqual_ThenReturnTrue()
            {
                var sut1 = SubRipTimeSpan.MaxValue();
                var sut2 = SubRipTimeSpan.MaxValue();

                var result = sut1.Equals(sut2);

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenTwoAreNotEqual_ThenReturnFalse()
            {
                var sut1 = SubRipTimeSpan.MaxValue();
                var sut2 = SubRipTimeSpan.MinValue();

                var result = sut1.Equals(sut2);

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class EqualsOperator : SubRipTimeSpanTests
        {
            [Test]
            public void WhenTwoAreEqual_ThenReturnTrue()
            {
                var sut1 = SubRipTimeSpan.MaxValue();
                var sut2 = SubRipTimeSpan.MaxValue();

                var result = sut1 == sut2;

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenTwoAreNotEqual_ThenReturnFalse()
            {
                var sut1 = SubRipTimeSpan.MaxValue();
                var sut2 = SubRipTimeSpan.MinValue();

                var result = sut1 == sut2;

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class GreaterOpertator : SubRipTimeSpanTests
        {
            [Test]
            public void WhenOneIsGreaterThanOther_ThenReturnTrue()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = max > min;

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenOneIsNotGreaterThanOther_ThenReturnFalse()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = min > max;

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class LessOperator : SubRipTimeSpanTests
        {
            [Test]
            public void WhenOneIsLessThanOther_ThenReturnTrue()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = min < max;

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenOneIsNotLessThanOther_ThenReturnFalse()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = max < min;

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class GreaterOrEqualOpertator : SubRipTimeSpanTests
        {
            [Test]
            public void WhenOneIsGreaterThanOther_ThenReturnTrue()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = max >= min;

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenOneIsEqualOther_ThenReturnTrue()
            {
                var max1 = SubRipTimeSpan.MaxValue();
                var max2 = SubRipTimeSpan.MaxValue();

                var result = max1 >= max2;

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenOneIsLessThanOther_ThenReturnFalse()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = min >= max;

                Assert.That(result, Is.False);
            }
        }

        [TestFixture]
        public class LessOrEqualOperator : SubRipTimeSpanTests
        {
            [Test]
            public void WhenOneIsLessThanOther_ThenReturnTrue()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = min <= max;

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenOneIsEqualOther_ThenReturnTrue()
            {
                var max1 = SubRipTimeSpan.MaxValue();
                var max2 = SubRipTimeSpan.MaxValue();

                var result = max2 <= max1;

                Assert.That(result, Is.True);
            }

            [Test]
            public void WhenOneIsGreaterThanOther_ThenReturnFalse()
            {
                var max = SubRipTimeSpan.MaxValue();
                var min = SubRipTimeSpan.MinValue();

                var result = max <= min;

                Assert.That(result, Is.False);
            }
        }
    }
}
