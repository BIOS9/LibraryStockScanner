namespace RfidAssetReader3MTests
{
    using NUnit.Framework;
    using RfidAssetReader3M.ReaderCommunication;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    public class ReaderResponseTests
    {
        struct ChecksumTestItem
        {
            public byte[] response;
            public byte[] expectedChecksum;

            public ChecksumTestItem(byte[] response, byte[] expectedChecksum)
            {
                this.response = response;
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
        public void TestCommunicationType()
        {
            foreach (CommunicationType t in Enum.GetValues(typeof(CommunicationType)))
            {
                ReaderResponse rr = new ReaderResponse(new byte[] { (byte)t, 0x00, 0x05, 0xFE, 0x00, 0x07, 0xDA, 0x02 });
                Assert.AreEqual(t, rr.CommunicationType);
                Assert.AreEqual((byte)t, rr.FullResponse[0]);
            }
        }

        [Test]
        public void TestResponseLength()
        {
            for (int i = 0; i < 253; ++i)
            {
                for (int j = 0; j < 253; ++j)
                {
                    byte[] data = new byte[i + 5];
                    data[0] = (byte)CommunicationType.Operation;
                    data[2] = (byte)(j + 2);

                    if (i == j)
                    {
                        ReaderResponse rr = new ReaderResponse(data, ignoreChecksum: true);
                        Assert.AreEqual(data, rr.FullResponse.ToArray());
                    }
                    else
                    {
                        Assert.Throws<ArgumentException>(() =>
                        {
                            ReaderResponse rr = new ReaderResponse(data);
                        });
                    }
                }
            }

            // Data too long.
            Assert.Throws<ArgumentException>(() =>
            {
                new ReaderResponse(new byte[259], ignoreChecksum: true);
            });
        }

        [Test]
        public void TestResponseData()
        {
            for (int i = 5; i < 258; ++i)
            {
                Span<byte> data = new byte[i];
                for (int j = 3; j < data.Length - 2; ++j)
                {
                    data[j] = (byte)j.GetHashCode(); // Just filling it with known stuff
                }
                data[0] = (byte)CommunicationType.Operation;
                data[2] = (byte)(i - 3);
                ReaderResponse rr = new ReaderResponse(data.ToArray(), ignoreChecksum: true);
                Assert.AreEqual(data.Slice(3, i - 5).ToArray(), rr.Response.ToArray());
            }
        }

        [Test]
        public void TestChecksum()
        {
            foreach (ChecksumTestItem item in testData)
            {
                byte[] fullResponse = new byte[item.response.Length + 5];
                fullResponse[0] = (byte)CommunicationType.Operation;
                fullResponse[2] = (byte)(item.response.Length + 2);
                Array.Copy(item.response, 0, fullResponse, 3, item.response.Length);
                Array.Copy(item.expectedChecksum, 0, fullResponse, fullResponse.Length - 2, 2);

                // Valid checksum
                ReaderResponse rr = new ReaderResponse(fullResponse);
                Assert.AreEqual(item.expectedChecksum, rr.Checksum.ToArray());
                Assert.IsTrue(rr.IsChecksumValid);
                rr = new ReaderResponse(fullResponse);
                Assert.AreEqual(item.expectedChecksum, rr.Checksum.ToArray());
                Assert.IsTrue(rr.IsChecksumValid);

                // Invalid checksum
                fullResponse[fullResponse.Length - 2] ^= 1;
                fullResponse[fullResponse.Length - 1] ^= 1;
                rr = new ReaderResponse(fullResponse, ignoreChecksum: true);
                Assert.IsFalse(rr.IsChecksumValid);
                Assert.Throws<DataException>(() => 
                {
                    new ReaderResponse(fullResponse);
                });
            }
        }

        [Test]
        public void TestFullResponse()
        {
            for (int i = 0; i < 260; ++i)
            {
                byte[] data = new byte[i];
                for (int j = 3; j < data.Length - 2; ++j)
                {
                    data[j] = (byte)j.GetHashCode(); // Just filling it with known stuff
                }
                if (data.Length >= 3)
                {
                    data[0] = (byte)CommunicationType.Operation;
                    data[2] = (byte)(i - 3);
                }

                if (i >= 5 && i <= 258)
                {
                    ReaderResponse rr = new ReaderResponse(data, ignoreChecksum: true);
                    Assert.AreEqual(data, rr.FullResponse.ToArray());
                } 
                else
                {
                    Assert.Throws<ArgumentException>(() =>
                    {
                        new ReaderResponse(data, ignoreChecksum: true);
                    });
                }
            }
        }
    }
}
