// -----------------------------------------------------------------------
// <copyright file="ReaderStreamTransceiver.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Transceivers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles conversion between stream of data between the application and
    /// reader to distinct commands and responses.
    /// </summary>
    /// <seealso cref="IReaderTransceiver"/>
    /// <seealso cref="ReaderCommand"/>
    /// <seealso cref="ReaderResponse"/>
    internal class ReaderStreamTransceiver : IReaderTransceiver, IDisposable, IAsyncDisposable
    {
        private readonly Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderStreamTransceiver"/> class.
        /// </summary>
        /// <param name="stream">RFID Reader stream to send and receive from.</param>
        public ReaderStreamTransceiver(Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable.", nameof(stream));
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException("Stream is not writable.", nameof(stream));
            }

            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <inheritdoc/>
        public bool IsReady => true;

        /// <inheritdoc/>
        public ReaderResponse Transceive(ReaderCommand command)
        {
            // Send command
            this.stream.Write(command.FullCommand);

            // Get response
            byte[] readBuffer = new byte[ReaderResponse.MaxFullResponseSize];
            int readCount = 0;

            // Read header
            while (readCount < 3)
            {
                readCount += this.stream.Read(readBuffer, 0, readBuffer.Length);
            }

            byte dataLength = readBuffer[2];

            // Read the data + checksum
            while (readCount < dataLength + 3)
            {
                readCount += this.stream.Read(readBuffer, dataLength, dataLength + 3 - readCount);
            }

            return null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.stream.Dispose();
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return this.stream.DisposeAsync();
        }
    }
}
