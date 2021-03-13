using System;
using System.Collections.Generic;
using ByteDev.Subtitles.SubRip.Validation;
using NUnit.Framework;

namespace ByteDev.Subtitles.SubRip.UnitTests.Validation
{
    [TestFixture]
    public class SubRipFileExtensionsTests
    {
        private const string FileName = "Test Video.mkv";

        private SubRipEntry _entry1;
        private SubRipEntry _entry2;
        private SubRipEntry _entry3;
        private SubRipEntry _entry4;
        private SubRipEntry _entry5;
        
        [SetUp]
        public void SetUp()
        {
            _entry1 = TestSubRipEntryFactory.Create1(); // "01:40:55,758 --> 01:40:58,426"
            _entry2 = TestSubRipEntryFactory.Create2(); // "01:40:58,677 --> 01:41:02,013"
            _entry3 = TestSubRipEntryFactory.Create3(); // "01:41:02,306 --> 01:41:04,640"
            _entry4 = TestSubRipEntryFactory.Create4(); // "01:41:04,808 --> 01:41:08,144"
            _entry5 = TestSubRipEntryFactory.Create5(); // "01:41:08,771 --> 01:41:12,106"
        }

        [TestFixture]
        public class IsOrderIdSequenceValid : SubRipFileExtensionsTests
        {
            [Test]
            public void WhenSourceIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = SubRipFileExtensions.IsOrderIdSequenceValid(null));
            }

            [Test]
            public void WhenWrongOrder_ThenReturnFalse()
            {
                var sut = new SubRipFile(FileName, new List<SubRipEntry>
                {
                    _entry1, _entry3, _entry2, _entry4, _entry5
                });

                var result = sut.IsOrderIdSequenceValid();

                Assert.That(result, Is.False);
            }

            [Test]
            public void WhenValidOrder_ThenReturnTrue()
            {
                var sut = new SubRipFile(FileName, new List<SubRipEntry>
                {
                    _entry1, _entry2, _entry3, _entry4, _entry5
                });

                var result = sut.IsOrderIdSequenceValid();

                Assert.That(result, Is.True);
            }
        }

        [TestFixture]
        public class AnyEntryOverlap : SubRipFileExtensionsTests
        {
            [Test]
            public void WhenIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = SubRipFileExtensions.AnyEntryOverlap(null));
            }

            [TestCase("01:41:08,771")]
            [TestCase("01:41:08,772")]
            public void WhenEntryOverlaps_ThenReturnTrue(string end)
            {
                // Entry 5 => "01:41:08,771 --> 01:41:12,106"
                _entry4.Duration.End = new SubRipTimeSpan(end);

                var sut = CreateSut();

                var result = sut.AnyEntryOverlap();

                Assert.That(result, Is.True);
            }

            [TestCase("01:41:08,769")]
            [TestCase("01:41:08,770")]
            public void WhenEntryDoesNotOverlap_ThenReturnFalse(string end)
            {
                // Entry 5 => "01:41:08,771 --> 01:41:12,106"
                _entry4.Duration.End = new SubRipTimeSpan(end);

                var sut = CreateSut();

                var result = sut.AnyEntryOverlap();

                Assert.That(result, Is.False);
            }

            private SubRipFile CreateSut()
            {
                return new SubRipFile(FileName, new List<SubRipEntry>
                {
                    _entry1, _entry2, _entry3, _entry4, _entry5
                });
            }
        }
    }
}