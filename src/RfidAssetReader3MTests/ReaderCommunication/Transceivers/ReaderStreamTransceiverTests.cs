

using RfidAssetReader3M.ReaderCommunication;
using RfidAssetReader3MTests.Helpers;

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

        [Test]
        public void TestNullTransceiveCommand()
        {
            Mock<Stream> mock = new Mock<Stream>();
            mock.Setup(m => m.CanWrite).Returns(true);
            mock.Setup(m => m.CanRead).Returns(true);

            ReaderStreamTransceiver t = new ReaderStreamTransceiver(mock.Object);

            Assert.Throws<ArgumentNullException>(() =>
            {
                t.Transceive(null);
            });
        }

        [Test]
        public void TestTransceiveCommand()
        {
            MemoryStream readStream = new MemoryStream(); // Stream that the transceiver reads from
            MemoryStream writeStream = new MemoryStream(); // Stream that the transceiver writes to
            BidirectionalMemoryStream stream = new BidirectionalMemoryStream(readStream, writeStream);

            ReaderStreamTransceiver t = new ReaderStreamTransceiver(stream);

            readStream.Write(new byte[] { 0xD6, 0x00, 0x07, 0xFE, 0x00, 0x00, 0x05, 0x00, 0xC9, 0x7B });

            ReaderCommand expectedCommand = new ReaderCommand(CommunicationType.Operation, new byte[] { 0xFE, 0x00, 0x07 });
            ReaderResponse response = t.Transceive(expectedCommand);

            // Assert reader response is read correctly
            Assert.AreEqual(new byte[] { 0xFE, 0x00, 0x00, 0x05, 0x00 }, response.Response.ToArray());
            Assert.IsTrue(response.IsChecksumValid);
            Assert.AreEqual(CommunicationType.Operation, response.CommunicationType);

            // Assert reader command is sent correctly
            byte[] buffer = new byte[expectedCommand.FullCommand.Length];
            Assert.AreEqual(buffer.Length, writeStream.Length);
            writeStream.Seek(-buffer.Length, SeekOrigin.End);
            writeStream.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(expectedCommand.FullCommand.ToArray(), buffer);
        }

        [Test]
        public void TestLargeTransceiveCommand()
        {
            MemoryStream readStream = new MemoryStream(); // Stream that the transceiver reads from
            MemoryStream writeStream = new MemoryStream(); // Stream that the transceiver writes to
            BidirectionalMemoryStream stream = new BidirectionalMemoryStream(readStream, writeStream, 10);

            ReaderStreamTransceiver t = new ReaderStreamTransceiver(stream);

            // Generate large reader command
            byte[] commandData = new byte[253];
            for (int i = 0; i < commandData.Length; ++i)
            {
                commandData[i] = (byte)(i.GetHashCode() ^ 1 & 0xFF);
            }
            ReaderCommand expectedCommand = new ReaderCommand(CommunicationType.Operation, commandData);

            // Generate large reader response using the reader command class
            byte[] responseData = new byte[253];
            for (int i = 0; i < responseData.Length; ++i)
            {
                responseData[i] = (byte)(i.GetHashCode() & 0xFF);
            }
            ReaderCommand expectedResponse = new ReaderCommand(CommunicationType.Operation, responseData);

            readStream.Write(expectedResponse.FullCommand);

            ReaderResponse response = t.Transceive(expectedCommand);

            // Assert reader response is read correctly
            Assert.AreEqual(expectedResponse.FullCommand.ToArray(), response.FullResponse.ToArray());

            // Assert reader command is sent correctly
            byte[] buffer = new byte[expectedCommand.FullCommand.Length];
            Assert.AreEqual(buffer.Length, writeStream.Length);
            writeStream.Seek(-buffer.Length, SeekOrigin.End);
            writeStream.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(expectedCommand.FullCommand.ToArray(), buffer);
        }
    }
}
