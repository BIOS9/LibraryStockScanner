// -----------------------------------------------------------------------
// <copyright file="ReaderCommand.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RfidAssetReader3MTests")]

namespace RfidAssetReader3M.ReaderCommunication
{
    using System;

    /// <summary>
    /// A command to send to the RFID reader.
    /// </summary>
    internal class ReaderCommand
    {
        private readonly byte[] fullCommand;
        private readonly byte[] command;
        private readonly byte[] checksum;
        private readonly CommunicationType communicationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderCommand"/> class.
        /// </summary>
        /// <param name="communicationType">The command type header byte.</param>
        /// <param name="command">Raw command to send to the reader.</param>
        public ReaderCommand(CommunicationType communicationType, byte[] command)
        {
            this.communicationType = communicationType;
            this.command = command ?? throw new ArgumentNullException(nameof(command));

            // Allocate space for full command.
            this.fullCommand = new byte[
                1 + // Communication type
                1 + // ¯\_(ツ)_/¯ Larger data length? Just a random space?
                1 + // Length value
                command.Length + // Command
                2]; // Checksum

            // Calculate length value
            // This value is used for Run-Length-Encoding while communicating with the reader.
            int length = command.Length + 2; // Length of command + 2 checksum bytes.

            // Ensure length can fit inside 1 byte.
            if (length > 255)
            {
                throw new ArgumentException("Command data is too long.", nameof(command));
            }

            // Populate full command.
            this.fullCommand[0] = (byte)communicationType;
            this.fullCommand[2] = (byte)length;
            Array.Copy(command, 0, this.fullCommand, 3, command.Length); // Copy command into fullCommand.

            // Calculate checksum of the command.
            Span<byte> checksumData = this.fullCommand;
            checksumData = checksumData.Slice(1); // Skip the communication type.
            checksumData = checksumData.Slice(0, checksumData.Length - 2); // Skip final 2 checksum bytes.
            Span<byte> checksumResult = BitConverter.GetBytes(Helpers.Checksum.CalculateCrc16(checksumData));
            checksumResult.Reverse(); // Swap endian
            this.checksum = checksumResult.ToArray();
            Array.Copy(this.checksum, 0, this.fullCommand, this.fullCommand.Length - 2, 2); // Copy checksum into fullCommand.
        }

        /// <summary>
        /// Gets the communication type header byte.
        /// </summary>
        public CommunicationType CommunicationType => this.communicationType;

        /// <summary>
        /// Gets the raw command to send to the reader.
        /// This does not include any of the header bytes or the checksum.
        /// </summary>
        public ReadOnlySpan<byte> Command => this.command;

        /// <summary>
        /// Gets the modified CRC16-CCITT checksum of the data.
        /// This checksum matches the format expected by the 3M RFID reader.
        /// </summary>
        public ReadOnlySpan<byte> Checksum => this.checksum;

        /// <summary>
        /// Gets the full command containing all header information and the checksum.
        /// The data is in the format expected by the 3M RFID reader.
        /// </summary>
        public ReadOnlySpan<byte> FullCommand => this.fullCommand;
    }
}
