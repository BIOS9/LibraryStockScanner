using System;
using System.IO;

namespace RfidAssetReader3MTests.Helpers
{
    public class BidirectionalMemoryStream : Stream
    {
        private MemoryStream readStream, writeStream;
        private long readPosition = 0;
        private long writePosition = 0;
        private int enforcedReadCount;

        public BidirectionalMemoryStream(MemoryStream readStream, MemoryStream writeStream, int enforcedReadCount = -1)
        {
            this.readStream = readStream;
            this.writeStream = writeStream;
            this.enforcedReadCount = enforcedReadCount;
        }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (enforcedReadCount > 0)
                count = Math.Min(enforcedReadCount, count);
            this.readStream.Seek(this.readPosition, SeekOrigin.Begin);
            int read = this.readStream.Read(buffer, offset, count);
            this.readPosition = this.readStream.Position;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.writeStream.Seek(this.writePosition, SeekOrigin.Begin);
            this.writeStream.Write(buffer, offset, count);
            this.writePosition = this.writeStream.Position;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new System.NotImplementedException();
        public override long Position
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }
    }
}