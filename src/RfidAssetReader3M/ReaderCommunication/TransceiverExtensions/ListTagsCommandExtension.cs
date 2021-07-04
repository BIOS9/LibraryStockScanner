// -----------------------------------------------------------------------
// <copyright file="ListTagsCommandExtension.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.TransceiverExtensions
{
    using RfidAssetReader3M.ReaderCommunication.Commands;
    using RfidAssetReader3M.ReaderCommunication.Responses;
    using RfidAssetReader3M.ReaderCommunication.Transceivers;

    /// <summary>
    /// Adds extension methods to transceivers to provide an easy way send and receive list tag commands.
    /// </summary>
    internal static class ListTagsCommandExtension
    {
        /// <summary>
        /// Send a <see cref="ListTagsCommand"/> to a transceiver and return <see cref="ListTagsResponse"/>.
        /// </summary>
        /// <param name="transceiver">Transceiver that is connected to an RFID reader.</param>
        /// <param name="command">Command to send to the reader.</param>
        /// <returns>Response from the reader.</returns>
        public static ListTagsResponse GetListTagsResponse(this IReaderTransceiver transceiver, ListTagsCommand command)
        {
            return (ListTagsResponse)transceiver.Transceive(command, ListTagsResponse.Factory);
        }
    }
}