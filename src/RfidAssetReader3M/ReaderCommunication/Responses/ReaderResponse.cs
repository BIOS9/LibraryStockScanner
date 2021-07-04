// -----------------------------------------------------------------------
// <copyright file="ReaderResponse.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Responses
{
    using System;

    /// <summary>
    /// A command response sent back from the reader.
    /// </summary>
    internal abstract class ReaderResponse
    {
        /// <summary>
        /// Gets the communication type header byte.
        /// </summary>
        public abstract CommunicationType CommunicationType { get; }

        /// <summary>
        /// Gets the raw command to send to the reader.
        /// This does not include any of the header bytes or the checksum.
        /// </summary>
        public abstract ReadOnlySpan<byte> Response { get; }

        /// <summary>
        /// Gets the modified CRC16-CCITT checksum of the data.
        /// This checksum matches the format expected by the 3M RFID reader.
        /// </summary>
        public abstract ReadOnlySpan<byte> Checksum { get; }

        /// <summary>
        /// Gets the full response from the reader including checksum and header.
        /// </summary>
        public abstract ReadOnlySpan<byte> FullResponse { get; }

        /// <summary>
        /// Gets a value indicating whether the provided checksum matches the calculated
        /// checksum of the data.
        /// </summary>
        public abstract bool IsChecksumValid { get; }
    }
}
