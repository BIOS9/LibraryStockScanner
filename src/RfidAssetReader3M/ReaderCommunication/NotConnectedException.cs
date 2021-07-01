// -----------------------------------------------------------------------
// <copyright file="NotConnectedException.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication
{
    using System;

    /// <summary>
    /// The exception that is thrown when an attempt is made to
    /// interact with a reader that is not connected.
    /// </summary>
    public class NotConnectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        public NotConnectedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public NotConnectedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public NotConnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
