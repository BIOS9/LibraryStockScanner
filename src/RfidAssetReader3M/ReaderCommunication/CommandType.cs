// -----------------------------------------------------------------------
// <copyright file="CommandType.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication
{
    /// <summary>
    /// The type of command.
    /// The reader expects different header values for different command types.
    /// </summary>
    public enum CommandType : byte
    {
        /// <summary>
        /// Used in the initial setup of the reader.
        /// </summary>
        Setup = 0xD5,

        /// <summary>
        /// Used in the normal operation of the reader to read and write tags.
        /// </summary>
        Operation = 0xD6,
    }
}
