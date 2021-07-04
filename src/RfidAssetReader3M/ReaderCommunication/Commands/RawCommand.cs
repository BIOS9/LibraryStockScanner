// -----------------------------------------------------------------------
// <copyright file="RawCommand.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Commands
{
    using System;
    using RfidAssetReader3M.ReaderCommunication.Responses;
    using RfidAssetReader3M.ReaderCommunication.Transceivers;

    /// <summary>
    /// Represents a command object that handles header and checksum data.
    /// </summary>
    internal class RawCommand : ReaderCommand
    {
        private readonly byte[] fullCommand;
        private readonly byte[] command;
        private readonly byte[] checksum;
        private readonly CommunicationType communicationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawCommand"/> class.
        /// </summary>
        /// <param name="communicationType">The command type header byte.</param>
        /// <param name="command">Raw command to send to the reader.</param>
        public RawCommand(CommunicationType communicationType, Span<byte> command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            this.communicationType = communicationType;
            this.command = command.ToArray(); // Defensive cloning of command.

            // Allocate space for full command.
            this.fullCommand = new byte[
                1 + // Communication type
                1 + // ¯\_(ツ)_/¯ Larger data length? Just a random space?
                1 + // Length value
                this.command.Length + // Command
                2]; // Checksum

            // Calculate length value
            // This value is used for Run-Length-Encoding while communicating with the reader.
            int length = this.command.Length + 2; // Length of command + 2 checksum bytes.

            // Ensure length can fit inside 1 byte.
            if (length > 255)
            {
                throw new ArgumentException("Command data is too long.", nameof(command));
            }

            // Populate full command.
            this.fullCommand[0] = (byte)communicationType;
            this.fullCommand[2] = (byte)length;
            Array.Copy(this.command, 0, this.fullCommand, 3, this.command.Length); // Copy command into fullCommand.

            // Calculate checksum of the command.
            Span<byte> checksumData = this.fullCommand;
            checksumData = checksumData.Slice(1); // Skip the communication type.
            checksumData = checksumData.Slice(0, checksumData.Length - 2); // Skip final 2 checksum bytes.
            byte[] checksumResult = BitConverter.GetBytes(Helpers.Checksum.CalculateCrc16(checksumData));
            Array.Reverse(checksumResult); // Swap endian
            this.checksum = checksumResult;
            Array.Copy(this.checksum, 0, this.fullCommand, this.fullCommand.Length - 2, 2); // Copy checksum into fullCommand.
        }

        /// <inheritdoc/>
        public override CommunicationType CommunicationType => this.communicationType;

        /// <inheritdoc/>
        public override ReadOnlySpan<byte> Command => this.command;

        /// <inheritdoc/>
        public override ReadOnlySpan<byte> Checksum => this.checksum;

        /// <inheritdoc/>
        public override ReadOnlySpan<byte> FullCommand => this.fullCommand;

        /// <inheritdoc/>
        public override RawResponse Send(IReaderTransceiver transceiver)
        {
            return (RawResponse)transceiver.Transceive(this, RawResponse.Factory);
        }
    }
}