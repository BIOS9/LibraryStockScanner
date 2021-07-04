// -----------------------------------------------------------------------
// <copyright file="IResponseFactory.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Responses
{
    using System;

    /// <summary>
    /// Delegates instantiation of <see cref="ReaderResponse"/> away from the calling class.
    /// </summary>
    internal interface IResponseFactory
    {
        /// <summary>
        /// Creates a <see cref="ReaderResponse"/> using data from the reader.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <returns>A reader response containing the parsed data.</returns>
        public ReaderResponse CreateFromData(Span<byte> data);
    }
}