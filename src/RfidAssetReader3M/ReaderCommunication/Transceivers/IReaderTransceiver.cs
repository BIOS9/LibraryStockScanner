﻿// -----------------------------------------------------------------------
// <copyright file="IReaderTransceiver.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Transceivers
{
    using RfidAssetReader3M.ReaderCommunication.Commands;
    using RfidAssetReader3M.ReaderCommunication.Responses;

    /// <summary>
    /// Handles low level communication between the application and a 3M RFID reader.
    /// </summary>
    internal interface IReaderTransceiver
    {
        /// <summary>
        /// Gets a value indicating whether the channel is ready to communicate.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Communicates with a reader by sending a command and then reading the response.
        /// </summary>
        /// <param name="command">Command that will be sent to the reader.</param>
        /// <param name="responseFactory">Response factory that will be used to create a response object.</param>
        /// <returns>The response to the command that was sent to the reader.</returns>
        ReaderResponse Transceive(ReaderCommand command, IResponseFactory responseFactory);
    }
}
