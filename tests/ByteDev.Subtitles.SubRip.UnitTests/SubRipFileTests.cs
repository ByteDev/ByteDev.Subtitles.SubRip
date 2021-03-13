using System;
using System.Collections.Generic;
using System.Linq;
using ByteDev.Collections;
using NUnit.Framework;

namespace ByteDev.Subtitles.SubRip.UnitTests
{
    [TestFixture]
    public class SubRipFileTests
    {
        private const string FileName = "Test Video.mkv";

        [TestFixture]
        public class Constructor_Strings : SubRipFileTests
        {
            [TestCase(null)]
            [TestCase("")]
            [TestCase(" ")]
            [TestCase(" \r\n")]
            public void WhenFileContentsNullOrEmptyOrWhiteSpace_ThenSetEmptyEntries(string fileContents)
            {
                var sut = new SubRipFile(FileName, fileContents);

                Assert.That(sut.Entries, Is.Empty);
            }
        }

        [TestFixture]
        public class Constructr_List : SubRipFileTests
        {
            [Test]
            public void WhenEntriesIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = new SubRipFile(FileName, null as List<SubRipEntry>));
            }
        }

        [TestFixture]
        public class Load : SubRipFileTests
        {
            [Test]
            public void WhenPathIsNull_ThenThrowException()
            {
                Assert.Throws<ArgumentNullException>(() => _ = SubRipFile.Load(null));
            }
        }

        [TestFixture]
        public class Sort : SubRipFileTests
        {
            [Test]
            public void WhenNoEntries_ThenDoNothing()
            {
                var sut = new SubRipFile(FileName, new List<SubRipEntry>());

                sut.Sort();

                Assert.That(sut.Entries, Is.Empty);
            }

            [Test]
            public void WhenOneEntry_ThenDoNothing()
            {
                var subRipEntry = TestSubRipEntryFactory.Create1();

                var sut = new SubRipFile(FileName, new List<SubRipEntry>
                {
                    subRipEntry
                });

                sut.Sort();

                Assert.That(sut.Entries.First().Text, Is.EqualTo(subRipEntry.Text));
            }

            [Test]
            public void WhenEntriesInNonSequentialOrder_ThenSort()
            {
                var entry1 = TestSubRipEntryFactory.Create1();
                var entry2 = TestSubRipEntryFactory.Create2();
                var entry3 = TestSubRipEntryFactory.Create3();
                var entry4 = TestSubRipEntryFactory.Create4();
                var entry5 = TestSubRipEntryFactory.Create5();

                var entries = new List<SubRipEntry>
                {
                    entry2, entry1, entry3, entry5, entry4
                };

                var sut = new SubRipFile(FileName, entries);

                sut.Sort();

                Assert.That(sut.Entries.First().OrderId, Is.EqualTo(entry1.OrderId));
                Assert.That(sut.Entries.First().Text, Is.EqualTo(entry1.Text));

                Assert.That(sut.Entries.Second().OrderId, Is.EqualTo(entry2.OrderId));
                Assert.That(sut.Entries.Second().Text, Is.EqualTo(entry2.Text));

                Assert.That(sut.Entries.Third().OrderId, Is.EqualTo(entry3.OrderId));
                Assert.That(sut.Entries.Third().Text, Is.EqualTo(entry3.Text));

                Assert.That(sut.Entries.Fourth().OrderId, Is.EqualTo(entry4.OrderId));
                Assert.That(sut.Entries.Fourth().Text, Is.EqualTo(entry4.Text));

                Assert.That(sut.Entries.Fifth().OrderId, Is.EqualTo(entry5.OrderId));
                Assert.That(sut.Entries.Fifth().Text, Is.EqualTo(entry5.Text));
            }
        }

        [TestFixture]
        public class SetOrderIds : SubRipFileTests
        {
            [Test]
            public void WhenNoEntries_ThenDoNothing()
            {
                var sut = new SubRipFile(FileName, new List<SubRipEntry>());

                sut.SetOrderIds();

                Assert.That(sut.Entries, Is.Empty);
            }

            [Test]
            public void WhenOneEntry_ThenDoNothing()
            {
                var subRipEntry = TestSubRipEntryFactory.Create1();

                var sut = new SubRipFile(FileName, new List<SubRipEntry>
                {
                    subRipEntry
                });

                sut.SetOrderIds();

                Assert.That(sut.Entries.First().Text, Is.EqualTo(subRipEntry.Text));
            }

            [Test]
            public void WhenEntriesInNonSequentialOrder_ThenReNumber()
            {
                var entry1 = TestSubRipEntryFactory.Create1();
                var entry2 = TestSubRipEntryFactory.Create2();
                var entry3 = TestSubRipEntryFactory.Create3();
                var entry4 = TestSubRipEntryFactory.Create4();
                var entry5 = TestSubRipEntryFactory.Create5();

                var entries = new List<SubRipEntry>
                {
                    entry2, entry1, entry3, entry5, entry4
                };

                var sut = new SubRipFile(FileName, entries);

                sut.SetOrderIds();

                Assert.That(sut.Entries.First().OrderId, Is.EqualTo(1));
                Assert.That(sut.Entries.First().Text, Is.EqualTo(entry2.Text));

                Assert.That(sut.Entries.Second().OrderId, Is.EqualTo(2));
                Assert.That(sut.Entries.Second().Text, Is.EqualTo(entry1.Text));

                Assert.That(sut.Entries.Third().OrderId, Is.EqualTo(3));
                Assert.That(sut.Entries.Third().Text, Is.EqualTo(entry3.Text));

                Assert.That(sut.Entries.Fourth().OrderId, Is.EqualTo(4));
                Assert.That(sut.Entries.Fourth().Text, Is.EqualTo(entry5.Text));

                Assert.That(sut.Entries.Fifth().OrderId, Is.EqualTo(5));
                Assert.That(sut.Entries.Fifth().Text, Is.EqualTo(entry4.Text));
            }
        }

        [TestFixture]
        public class SetAbsoluteDuration : SubRipFileTests
        {
            private SubRipFile _sut;

            [SetUp]
            public void SetUp()
            {
                // "01:40:55,758 --> 01:40:58,426"
                var entry1 = TestSubRipEntryFactory.Create1();

                // "01:40:58,677 --> 01:41:02,013"
                var entry2 = TestSubRipEntryFactory.Create2();

                var entries = new List<SubRipEntry> { entry1, entry2 };

                _sut = new SubRipFile(FileName, entries);
            }

            [Test]
            public void WhenMultipleEntries_ThenSetDurationOnAll()
            {
                var ts = new SubRipTimeSpan(0, 0, 2, 100);

                _sut.SetAbsoluteDuration(ts);

                Assert.That(_sut.Entries.First().Duration.ToString(), Is.EqualTo("01:40:55,758 --> 01:40:57,858"));
                Assert.That(_sut.Entries.Second().Duration.ToString(), Is.EqualTo("01:40:58,677 --> 01:41:00,777"));
            }

            [Test]
            public void WhenOverlapsWithNextEntry_ThenSetToStartOfNext()
            {
                _sut.Entries.First().Duration = new SubRipDuration("01:40:00,000 --> 01:40:02,500");
                _sut.Entries.Second().Duration = new SubRipDuration("01:40:03,000 --> 01:40:05,000");

                var ts = new SubRipTimeSpan(0, 0, 4, 0);

                _sut.SetAbsoluteDuration(ts, true);

                Assert.That(_sut.Entries.First().Duration.ToString(), Is.EqualTo("01:40:00,000 --> 01:40:02,999"));
                Assert.That(_sut.Entries.Second().Duration.ToString(), Is.EqualTo("01:40:03,000 --> 01:40:07,000"));
            }
        }

        [TestFixture]
        public class SetMaxDuration : SubRipFileTests
        {
            private SubRipEntry _entry1;
            private SubRipEntry _entry2;

            [SetUp]
            public void SetUp()
            {
                // "01:40:55,758 --> 01:40:58,426" => Duration = {00:00:02,668}
                _entry1 = TestSubRipEntryFactory.Create1(); 

                // "01:40:58,677 --> 01:41:02,013" => Duration = {00:00:03,336}
                _entry2 = TestSubRipEntryFactory.Create2();
            }

            [Test]
            public void WhenEntryOverMax_ThenSetDurationToMax()
            {
                var ts = new SubRipTimeSpan(0, 0, 2, 500);

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.SetMaxDuration(ts);

                Assert.That(sut.Entries.Single().Duration.ToString(), Is.EqualTo("01:40:55,758 --> 01:40:58,258"));
            }

            [Test]
            public void WhenEntryLessThanMax_ThenDoNothing()
            {
                var ts = new SubRipTimeSpan(0, 0, 2, 700);

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.SetMaxDuration(ts);

                Assert.That(sut.Entries.Single().Duration.ToString(), Is.EqualTo("01:40:55,758 --> 01:40:58,426"));
            }

            [Test]
            public void WhenEntriesMix_ThenSetDuration()
            {
                var ts = new SubRipTimeSpan(0, 0, 2, 800);

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1, _entry2 });

                sut.SetMaxDuration(ts);

                // Hasn't changed
                Assert.That(sut.Entries.First().Duration.ToString(), Is.EqualTo("01:40:55,758 --> 01:40:58,426"));

                // Has set to new ts
                Assert.That(sut.Entries.Second().Duration.ToString(), Is.EqualTo("01:40:58,677 --> 01:41:01,477"));
            }
        }

        [TestFixture]
        public class RemoveTextFormatting : SubRipFileTests
        {
            private SubRipEntry _entry1;
            private SubRipEntry _entry2;

            [SetUp]
            public void SetUp()
            {
                _entry1 = TestSubRipEntryFactory.Create1();
                _entry2 = TestSubRipEntryFactory.Create2();
            }

            [Test]
            public void WhenTextContainsBold_ThenRemove()
            {
                _entry1.Text = "This is some <b>text</b> and some other {b}text{/b}.";

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.RemoveTextFormatting();

                Assert.That(sut.Entries.Single().Text, Is.EqualTo("This is some text and some other text."));
            }

            [Test]
            public void WhenTextContainsItalics_ThenRemove()
            {
                _entry1.Text = "This is some <i>text</i> and some other {i}text{/i}.";

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.RemoveTextFormatting();

                Assert.That(sut.Entries.Single().Text, Is.EqualTo("This is some text and some other text."));
            }

            [Test]
            public void WhenTextContainsUnderLine_ThenRemove()
            {
                _entry1.Text = "This is some <u>text</u> and some other {u}text{/u}.";

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.RemoveTextFormatting();

                Assert.That(sut.Entries.Single().Text, Is.EqualTo("This is some text and some other text."));
            }

            [TestCase("This is some <font color=\"color name or #code\">text</font> string.", "This is some text string.")]
            [TestCase("This is some <font color=\"color name or #code\">text</font>", "This is some text")]
            [TestCase("<font color=\"color name or #code\">text</font>", "text")]
            public void WhenTextContainsFontColor_ThenRemove(string text, string expected)
            {
                _entry1.Text = text;

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.RemoveTextFormatting();

                Assert.That(sut.Entries.Single().Text, Is.EqualTo(expected));
            }
        }

        [TestFixture]
        public class SetTextLineMaxLength : SubRipFileTests
        {
            private SubRipEntry _entry1;
            private SubRipEntry _entry2;

            [SetUp]
            public void SetUp()
            {
                _entry1 = TestSubRipEntryFactory.Create1();
                _entry2 = TestSubRipEntryFactory.Create2();
            }

            [Test]
            public void WhenNumberOfCharsIsLessThanOne_ThenThrowException()
            {
                var sut = new SubRipFile(FileName, new List<SubRipEntry>());

                Assert.Throws<ArgumentOutOfRangeException>(() => sut.SetTextLineMaxLength(0));
            }

            [TestCase("", 1)]
            [TestCase("A", 1)]
            [TestCase("A", 2)]
            [TestCase("The", 3)]
            [TestCase("The", 4)]
            [TestCase("Hello World", 11)]
            [TestCase("Hello World", 12)]
            public void WhenTextLengthIsEqualOrLessThanMax_ThenDoNothing(string text, int maxLineLength)
            {
                _entry1.Text = text;

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.SetTextLineMaxLength(maxLineLength);

                Assert.That(sut.Entries.Single().Text, Is.EqualTo(text));
            }

            [Test]
            public void WhenTextLengthIsMoreThanMax_ThenReturnTwoLines()
            {               
                             // 12345678901234567890
                _entry1.Text = "This is some text thats just too long.";

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.SetTextLineMaxLength(20);

                Assert.That(sut.Entries.Single().Text, Is.EqualTo("This is some text\r\nthats just too long."));
            }

            [Test]
            public void WhenTextLengthIsMoreThanMax_ThenReturnThreeLines()
            {                                       
                             // 1234567890123456789012345678901234567890
                _entry1.Text = "This is some text thats just too long. Other text.";

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.SetTextLineMaxLength(20);

                Assert.That(sut.Entries.Single().Text, Is.EqualTo("This is some text\r\nthats just too long.\r\n Other text."));
            }

            [TestCase("This is some text\nthats just too long.")]
            [TestCase("This is some text\r\nthats just too long.")]
            public void WhenTextContainsReturnChars_ThenDoNothing(string text)
            {
                var entry = TestSubRipEntryFactory.Create1();

                entry.Text = text;

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { entry });

                sut.SetTextLineMaxLength(20);

                Assert.That(sut.Entries.Single().Text, Is.EqualTo(text));
            }
        }

        [TestFixture]
        public class RemoveTextReturnChars : SubRipFileTests
        {
            private SubRipEntry _entry1;
            private SubRipEntry _entry2;

            [SetUp]
            public void SetUp()
            {
                _entry1 = TestSubRipEntryFactory.Create1();
                _entry2 = TestSubRipEntryFactory.Create2();
            }

            [TestCase("Something\r\nand something else\r\n", "Something and something else")]
            [TestCase("Something\nand something else\n", "Something and something else")]
            public void WhenContainsReturns_ThenRemoveReturns(string text, string expected)
            {
                _entry1.Text = text;

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.RemoveTextReturnChars();

                Assert.That(sut.Entries.Single().Text, Is.EqualTo(expected));
            }

            [Test]
            public void WhenDoesNotContainReturnChars_ThenDoNothing()
            {
                _entry1.Text = "Some text";

                var sut = new SubRipFile(FileName, new List<SubRipEntry> { _entry1 });

                sut.RemoveTextReturnChars();

                Assert.That(sut.Entries.Single().Text, Is.EqualTo("Some text"));
            }
        }

        [TestFixture]
        public class RemoveEntry : SubRipFileTests
        {
            private SubRipEntry _entry1;
            private SubRipEntry _entry2;
            private SubRipEntry _entry3;

            [SetUp]
            public void SetUp()
            {
                _entry1 = TestSubRipEntryFactory.Create1();
                _entry2 = TestSubRipEntryFactory.Create2();
                _entry3 = TestSubRipEntryFactory.Create2();
            }

            [TestCase(-1)]
            [TestCase(0)]
            public void WhenOrderIdIsInvalid_ThenThrowException(int orderId)
            {
                var sut = CreateSut();

                Assert.Throws<ArgumentOutOfRangeException>(() => sut.RemoveEntry(orderId));
            }

            [Test]
            public void WhenDoesNotContainOrderId_ThenDoNothing()
            {
                var sut = CreateSut();

                sut.RemoveEntry(99);

                Assert.That(sut.Entries.Count, Is.EqualTo(3));
            }

            [Test]
            public void WhenContainsEntries_ThenDelete()
            {
                _entry1.OrderId = 1;
                _entry2.OrderId = 2;
                _entry3.OrderId = 1;

                var sut = CreateSut();

                sut.RemoveEntry(1);

                Assert.That(sut.Entries.Single().OrderId, Is.EqualTo(2));
            }

            private SubRipFile CreateSut()
            {
                return new SubRipFile(FileName, new List<SubRipEntry> { _entry1, _entry2, _entry3 });
            }
        }
    }
}