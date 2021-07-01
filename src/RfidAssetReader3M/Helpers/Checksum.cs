// -----------------------------------------------------------------------
// <copyright file="Checksum.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.Helpers
{
    using System;

    /// <summary>
    /// Contains helper functions for checksum calculation.
    /// </summary>
    internal static class Checksum
    {
        /// <summary>
        /// Calculates modified CRC16-CCITT.
        /// The final output of this CRC is XOR'ed with 0xFFFF.
        /// </summary>
        /// <param name="data">Input data for the checksum calculation.</param>
        /// <returns>16 bit unsigned integer checksum result.</returns>
        public static ushort CalculateCrc16(Span<byte> data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021); // 0x1021 CRC polynomial
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }

            crc ^= 0xFFFF; // XOR final output
            return crc;
        }
    }
}
