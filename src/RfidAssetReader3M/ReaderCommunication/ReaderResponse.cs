// -----------------------------------------------------------------------
// <copyright file="ReaderResponse.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication
{
    using System;
    using System.Data;

    /// <summary>
    /// A command response sent back from the reader.
    /// </summary>
    internal class ReaderResponse
    {
        private readonly byte[] fullResponse;
        private readonly byte[] response;
        private readonly byte[] checksum;
        private readonly CommunicationType communicationType;
        private readonly bool validChecksum;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderResponse"/> class.
        /// </summary>
        /// <param name="fullResponse">Full response from the reader including checksum and header.</param>
        /// <param name="ignoreChecksum">Allows continuing with an invalid checksum.</param>
        public ReaderResponse(byte[] fullResponse, bool ignoreChecksum = false)
        {
            this.fullResponse = fullResponse ?? throw new ArgumentNullException(nameof(fullResponse));

            // Ensure full response has at least 3 bytes for the header and 2 bytes for the checksum.
            if (fullResponse.Length < 5)
            {
                throw new ArgumentException("Full response must contain at least 5 bytes.", nameof(fullResponse));
            }

            // Ensure full response is not longer than the maximum size.
            // Max size is governed by the max data length (255) + the header (3)
            if (fullResponse.Length > 258)
            {
                throw new ArgumentException("Full response is too long.");
            }

            // Ensure communication type is an expected value.
            if (!Enum.IsDefined(typeof(CommunicationType), fullResponse[0]))
            {
                throw new ArgumentException("Unknown communication type.", nameof(fullResponse));
            }
            else
            {
                this.communicationType = (CommunicationType)fullResponse[0];
            }

            // Ensure that weird space byte is 0 until I figure out what it does.
            if (fullResponse[1] != 0)
            {
                throw new ArgumentException("Unexpected value for byte 2.", nameof(fullResponse));
            }

            byte dataLength = fullResponse[2];

            // Ensure data length is at least 2 since it includes the checksum.
            if (dataLength < 2)
            {
                throw new ArgumentException("Data length is less than 2. 2 bytes are required to fit the checksum.", nameof(fullResponse));
            }

            // Ensure length value matches length of given payload.
            if (dataLength != fullResponse.Length - 3)
            {
                throw new ArgumentException("Data length value does not match the length of the actual data.", nameof(fullResponse));
            }

            // Allocate and populate response data.
            this.response = new byte[dataLength - 2];
            Array.Copy(fullResponse, 3, this.response, 0, dataLength - 2);

            // Allocate and populate checksum.
            this.checksum = new byte[2];
            Array.Copy(fullResponse, fullResponse.Length - 2, this.checksum, 0, 2);

            // Calculate and validate checksum.
            this.validChecksum = this.ValidateChecksum();
            if (!ignoreChecksum && !this.validChecksum)
            {
                throw new DataException("Invalid checksum");
            }
        }

        /// <summary>
        /// Gets the communication type header byte.
        /// </summary>
        public CommunicationType CommunicationType => this.communicationType;

        /// <summary>
        /// Gets the raw command to send to the reader.
        /// This does not include any of the header bytes or the checksum.
        /// </summary>
        public ReadOnlySpan<byte> Response => this.response;

        /// <summary>
        /// Gets the modified CRC16-CCITT checksum of the data.
        /// This checksum matches the format expected by the 3M RFID reader.
        /// </summary>
        public ReadOnlySpan<byte> Checksum => this.checksum;

        /// <summary>
        /// Gets the full response from the reader including checksum and header.
        /// </summary>
        public ReadOnlySpan<byte> FullResponse => this.fullResponse;

        /// <summary>
        /// Gets a value indicating whether the provided checksum matches the calcluated
        /// checksum of the data.
        /// </summary>
        public bool IsChecksumValid => this.validChecksum;

        private bool ValidateChecksum()
        {
            // Calculate checksum of the command.
            Span<byte> checksumData = this.fullResponse;
            checksumData = checksumData.Slice(1); // Skip the communication type.
            checksumData = checksumData.Slice(0, checksumData.Length - 2); // Skip final 2 checksum bytes.
            Span<byte> checksumResult = BitConverter.GetBytes(Helpers.Checksum.CalculateCrc16(checksumData));
            checksumResult.Reverse(); // Swap endian
            return checksumResult.SequenceEqual(this.checksum);
        }
    }
}
