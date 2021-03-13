using System.IO;
using System.Linq;
using ByteDev.Collections;
using NUnit.Framework;

namespace ByteDev.Subtitles.SubRip.IntTests
{
    [TestFixture]
    public class SubRipFileTests
    {
        [TestFixture]
        public class Load : SubRipFileTests
        {
            [Test]
            public void WhenFileDoesNotExist_ThenThrowException()
            {
                Assert.Throws<FileNotFoundException>(() => SubRipFile.Load(TestFiles.DoesNotExist));
            }

            [Test]
            public void WhenFileExists_ThenLoadFromFile()
            {
                var sut = SubRipFile.Load(TestFiles.CarlitosWay);

                Assert.That(sut.FileName, Is.EqualTo("Carlito's Way [1993] (English Forced).srt"));
                Assert.That(sut.Entries.Count, Is.EqualTo(11));

                Assert.That(sut.Entries.First().OrderId, Is.EqualTo(1));
                Assert.That(sut.Entries.First().Duration.ToString(), Is.EqualTo("01:40:55,758 --> 01:40:58,426"));
                Assert.That(sut.Entries.First().Text, Is.EqualTo("Listen to me carefully, Carlito."));

                Assert.That(sut.Entries.Second().OrderId, Is.EqualTo(2));
                Assert.That(sut.Entries.Second().Duration.ToString(), Is.EqualTo("01:40:58,677 --> 01:41:02,013"));
                Assert.That(sut.Entries.Second().Text, Is.EqualTo("Rudy says Pachanga is complaining\r\nabout being broke;"));

                Assert.That(sut.Entries.Last().OrderId, Is.EqualTo(11));
                Assert.That(sut.Entries.Last().Duration.ToString(), Is.EqualTo("01:41:37,633 --> 01:41:39,634"));
                Assert.That(sut.Entries.Last().Text, Is.EqualTo("See you tomorrow."));
            }

            [Test]
            public void WhenPerformExtraValidation_AndFileInvalid_ThenThrowException()
            {
                var ex = Assert.Throws<SubRipException>(() => _ = SubRipFile.Load(TestFiles.WrongOrder, true));
                Assert.That(ex.Message, Is.EqualTo("SubRip file contains entries with Order ID in the wrong order/position. Order IDs should be sequential from one onwards."));
            }
        }
    }
}