namespace RfidAssetReader3MTests.ReaderCommunication
{
    using NUnit.Framework;
    using RfidAssetReader3M.ReaderCommunication;
    using System;
    using System.Collections.Generic;

    public class ReaderCommandTests
    {
        struct ChecksumTestItem
        {
            public byte[] command;
            public byte[] expectedChecksum;

            public ChecksumTestItem(byte[] command, byte[] expectedChecksum)
            {
                this.command = command;
                this.expectedChecksum = expectedChecksum;
            }
        }

        List<ChecksumTestItem> testData = new List<ChecksumTestItem>
        {
            new ChecksumTestItem(
                new byte[] { 0xFE, 0x00, 0x07 },
                new byte[] { 0xDA, 0x02 }),
            new ChecksumTestItem(
                new byte[] { 0x04, 0x00, 0x11, 0x0a, 0x05, 0x00, 0x02 },
                new byte[] { 0x72, 0x50 }),
            new ChecksumTestItem(
                new byte[] { 0xFE, 0x00, 0x00, 0x05, 0x00 },
                new byte[] { 0xC9, 0x7B }),
            new ChecksumTestItem(
                new byte[] { 0xFE, 0x00, 0x00, 0x05, 0x01, 0xE0, 0x04, 0x01, 0x00, 0x31, 0x23, 0xAA, 0x26 },
                new byte[] { 0x94, 0x1A }),
            new ChecksumTestItem(
                new byte[] { 0xFE, 0x00, 0x00, 0x05, 0x01, 0xE0, 0x04, 0x01, 0x00, 0x01, 0x7C, 0x0C, 0x38 },
                new byte[] { 0x8E, 0x2B }),
            new ChecksumTestItem(
                new byte[] { 0xFE, 0x00, 0x00, 0x05, 0x01, 0xE0, 0x04, 0x01, 0x00, 0x31, 0x23, 0xAA, 0x28 },
                new byte[] { 0x75, 0xD4 }),
            new ChecksumTestItem(
                new byte[] { 0xFE, 0x00, 0x00, 0x05, 0x02, 0xE0, 0x04, 0x01, 0x00, 0x31, 0x23, 0xAA, 0x26, 0xE0, 0x04, 0x01, 0x00, 0x01, 0x7C, 0x0C, 0x38 },
                new byte[] { 0xCA, 0xDB }),
            new ChecksumTestItem(
                new byte[] { 0xFE, 0x00, 0x00, 0x05, 0x02, 0xE0, 0x04, 0x01, 0x00, 0x31, 0x23, 0xAA, 0x26, 0xE0, 0x04, 0x01, 0x00, 0x31, 0x23, 0xAA, 0x28 },
                new byte[] { 0x31, 0x24 }),
        };

        [Test]
        public void TestNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ReaderCommand(CommunicationType.Operation, null);
            });
        }

        [Test]
        public void TestCommunicationType()
        {
            foreach (CommunicationType t in Enum.GetValues(typeof(CommunicationType)))
            {
                ReaderCommand rc = new ReaderCommand(t, new byte[0]);
                Assert.AreEqual(t, rc.CommunicationType);
                Assert.AreEqual((byte)t, rc.FullCommand[0]);
            }
        }

        [Test]
        public void TestCommandLength()
        {
            for (int i = 0; i < 253; ++i)
            {
                byte[] data = new byte[i];
                ReaderCommand rc = new ReaderCommand(CommunicationType.Operation, data);
                Assert.AreEqual(0, rc.FullCommand[1]);
                Assert.AreEqual(i + 2, rc.FullCommand[2]);
                Assert.AreEqual(data, rc.Command.ToArray());
                Assert.AreEqual(data, rc.FullCommand.Slice(3, data.Length).ToArray());
                Assert.AreEqual(data.Length + 2 + 3, rc.FullCommand.Length);
            }

            Assert.Throws<ArgumentException>(() =>
            {
                new ReaderCommand(CommunicationType.Operation, new byte[254]);
            });
        }

        [Test]
        public void TestCommandData()
        {
            for (int i = 0; i < 253; ++i)
            {
                byte[] data = new byte[i];
                for (int j = 0; j < data.Length; ++j)
                {
                    data[j] = (byte)j.GetHashCode(); // Just filling it with known stuff
                }
                ReaderCommand rc = new ReaderCommand(CommunicationType.Operation, data);
                Assert.AreEqual(0, rc.FullCommand[1]);
                Assert.AreEqual(data, rc.Command.ToArray());
                Assert.AreEqual(data, rc.FullCommand.Slice(3, data.Length).ToArray());
            }
        }

        [Test]
        public void TestChecksum()
        {
            foreach (ChecksumTestItem item in testData)
            {
                ReaderCommand rc = new ReaderCommand(CommunicationType.Operation, item.command);
                Assert.AreEqual(item.expectedChecksum, rc.Checksum.ToArray());
                Assert.AreEqual(item.expectedChecksum, rc.FullCommand.Slice(rc.FullCommand.Length - 2, 2).ToArray());
            }
        }

        [Test]
        public void TestFullCommand()
        {
            foreach (ChecksumTestItem item in testData)
            {
                ReaderCommand rc = new ReaderCommand(CommunicationType.Operation, item.command);
                byte[] expected = new byte[3 + item.command.Length + 2];
                expected[0] = 0xD6;
                expected[2] = (byte)(item.command.Length + 2);
                Array.Copy(item.command, 0, expected, 3, item.command.Length);
                Array.Copy(item.expectedChecksum, 0, expected, expected.Length - 2, 2);
                Assert.AreEqual(expected, rc.FullCommand.ToArray());
            }
        }
    }
}