// -----------------------------------------------------------------------
// <copyright file="ListTagsResponse.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Responses
{
    using System;
    using RfidAssetReader3M.ReaderCommunication.Commands;

    /// <summary>
    /// A response to a <see cref="ListTagsCommand"/>.
    /// </summary>
    internal class ListTagsResponse : RawResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListTagsResponse"/> class.
        /// </summary>
        /// <param name="fullResponse">Full response from the RFID reader.</param>
        public ListTagsResponse(Span<byte> fullResponse)
            : base(fullResponse)
        {
        }

        /// <summary>
        /// Gets an instance of a factory that creates a new instance of <see cref="ListTagsResponse"/>.
        /// </summary>
        public static new IResponseFactory Factory => new ListTagsResponseFactory();

        private class ListTagsResponseFactory : IResponseFactory
        {
            public ReaderResponse CreateFromData(Span<byte> data)
            {
                return new ListTagsResponse(data);
            }
        }
    }
}