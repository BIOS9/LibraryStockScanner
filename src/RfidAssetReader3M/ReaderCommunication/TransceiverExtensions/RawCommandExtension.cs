// -----------------------------------------------------------------------
// <copyright file="RawCommandExtension.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.TransceiverExtensions
{
    using RfidAssetReader3M.ReaderCommunication.Commands;
    using RfidAssetReader3M.ReaderCommunication.Responses;
    using RfidAssetReader3M.ReaderCommunication.Transceivers;

    /// <summary>
    /// Adds extension methods to transceivers to provide an easy way send and receive raw commands.
    /// </summary>
    internal static class RawCommandExtension
    {
        /// <summary>
        /// Send a <see cref="RawCommand"/> to a transceiver and return <see cref="RawResponse"/>.
        /// </summary>
        /// <param name="transceiver">Transceiver that is connected to an RFID reader.</param>
        /// <param name="command">Command to send to the reader.</param>
        /// <returns>Response from the reader.</returns>
        public static RawResponse GetRawResponse(this IReaderTransceiver transceiver, RawCommand command)
        {
            return (RawResponse)transceiver.Transceive(command, RawResponse.Factory);
        }
    }
}