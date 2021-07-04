// -----------------------------------------------------------------------
// <copyright file="ListTagsCommand.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Commands
{
    using RfidAssetReader3M.ReaderCommunication.Responses;
    using RfidAssetReader3M.ReaderCommunication.Transceivers;

    /// <summary>
    /// Represents a command that requests the RFID reader to return all present tags.
    /// </summary>
    internal class ListTagsCommand : RawCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListTagsCommand"/> class.
        /// </summary>
        public ListTagsCommand()
            : base(CommunicationType.Operation, new byte[] { })
        {
        }
    }
}