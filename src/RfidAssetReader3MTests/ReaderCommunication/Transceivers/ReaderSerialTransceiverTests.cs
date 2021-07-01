namespace RfidAssetReader3MTests.ReaderCommunication.Transceivers
{
    using NUnit.Framework;
    using RfidAssetReader3M.ReaderCommunication.Transceivers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ReaderSerialTransceiverTests
    {
        [Test]
        public void TestIsReady()
        {
            Assert.IsTrue(new ReaderStreamTransceiver(new MemoryStream()).IsReady);
        }
    }
}
