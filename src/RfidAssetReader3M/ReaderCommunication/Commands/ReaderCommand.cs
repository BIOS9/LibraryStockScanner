// -----------------------------------------------------------------------
// <copyright file="ReaderCommand.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Commands
{
    using System;
    using RfidAssetReader3M.ReaderCommunication.Responses;
    using RfidAssetReader3M.ReaderCommunication.Transceivers;

    /// <summary>
    /// A command to send to the RFID reader.
    /// </summary>
    internal abstract class ReaderCommand
    {
        /// <summary>
        /// Gets the communication type header byte.
        /// </summary>
        public abstract CommunicationType CommunicationType { get; }

        /// <summary>
        /// Gets the raw command to send to the reader.
        /// This does not include any of the header bytes or the checksum.
        /// </summary>
        public abstract ReadOnlySpan<byte> Command { get; }

        /// <summary>
        /// Gets the modified CRC16-CCITT checksum of the data.
        /// This checksum matches the format expected by the 3M RFID reader.
        /// </summary>
        public abstract ReadOnlySpan<byte> Checksum { get; }

        /// <summary>
        /// Gets the full command containing all header information and the checksum.
        /// The data is in the format expected by the 3M RFID reader.
        /// </summary>
        public abstract ReadOnlySpan<byte> FullCommand { get; }
    }
}
