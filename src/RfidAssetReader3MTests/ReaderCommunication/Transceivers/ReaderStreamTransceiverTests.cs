

namespace RfidAssetReader3MTests.ReaderCommunication.Transceivers
{
    using Moq;
    using NUnit.Framework;
    using RfidAssetReader3M.ReaderCommunication.Transceivers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ReaderStreamTransceiverTests
    {
        [Test]
        public void TestIsReady()
        {
            Assert.IsTrue(new ReaderStreamTransceiver(new MemoryStream()).IsReady);
        }

        [Test]
        public void TestUnwritableStream()
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(m => m.CanRead).Returns(true);

            mock.Setup(m => m.CanWrite).Returns(true);
            new ReaderStreamTransceiver(mock.Object);

            mock.Setup(m => m.CanWrite).Returns(false);
            Assert.Throws<ArgumentException>(() =>
            {
                new ReaderStreamTransceiver(mock.Object);
            });
        }

        [Test]
        public void TestUnreadableStream()
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(m => m.CanWrite).Returns(true);

            mock.Setup(m => m.CanRead).Returns(true);
            new ReaderStreamTransceiver(mock.Object);

            mock.Setup(m => m.CanRead).Returns(false);
            Assert.Throws<ArgumentException>(() =>
            {
                new ReaderStreamTransceiver(mock.Object);
            });
        }

        [Test]
        public void TestDispose()
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(m => m.CanWrite).Returns(true);
            mock.Setup(m => m.CanRead).Returns(true);

            mock.Setup(m => m.Close()).Verifiable(); // Cant mock dispose, why?

            ReaderStreamTransceiver t = new ReaderStreamTransceiver(mock.Object);
            t.Dispose();

            mock.Verify();
        }

        [Test]
        public void TestDisposeAsync()
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(m => m.CanWrite).Returns(true);
            mock.Setup(m => m.CanRead).Returns(true);

            mock.Setup(m => m.DisposeAsync()).Verifiable();

            ReaderStreamTransceiver t = new ReaderStreamTransceiver(mock.Object);
            t.DisposeAsync().AsTask().Wait();

            mock.Verify();
        }
    }
}
