// -----------------------------------------------------------------------
// <copyright file="IAssetReader.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AssetReader.Abstractions
{
    /// <summary>
    /// Represents a device that reads or scans for assets.
    /// An example of such a device might be a barcode scanner or
    /// an RFID reader that can scan IDs of books in a library.
    /// </summary>
    public interface IAssetReader
    {
        /// <summary>
        /// Checks if the reader is ready to read assets.
        /// </summary>
        /// <returns>Returns true if the reader is ready.</returns>
        bool IsReady();
    }
}
