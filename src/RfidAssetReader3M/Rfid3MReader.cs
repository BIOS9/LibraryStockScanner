// -----------------------------------------------------------------------
// <copyright file="Rfid3MReader.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RfidAssetReader3MTests")]

namespace RfidAssetReader3M
{
    using System;
    using AssetReader.Abstractions;

    /// <summary>
    /// Reads ISO/IEC 15693 asset tags using a 3M 810 RFID reader.
    /// </summary>
    internal class Rfid3MReader : IAssetReader
    {
        private enum MessageType : byte
        {
            ListTags = 0xFE,
            ReadTag = 0x02,
        }

        /// <inheritdoc/>
        public bool IsReady()
        {
            throw new NotImplementedException();
        }

        // public LibraryStockScanner()
        // {
        //    _serial = new SerialPort("COM6", 19200, Parity.None, 8, StopBits.One);
        //    _serial.Open();
        //    Console.WriteLine("Opened");
        //    IDictionary<string, string> books = new Dictionary<string, string>();
        //    while (true)
        //    {
        //        List<Tag> tags = GetPresentTags();
        //        //Console.WriteLine($"Found {tags.Count} tags.");
        //        foreach (Tag tag in tags)
        //        {
        //            if (!books.ContainsKey(tag.ToString()))
        //            {
        //                string bookID = getTagData(tag);
        //                if (string.IsNullOrWhiteSpace(bookID))
        //                {
        //                    continue;
        //                }
        //                books.Add(tag.ToString(), bookID);
        //                Console.WriteLine($"New book: {bookID}");
        //            }
        //        }
        //    }
        // }

        /*
    private List<Tag> GetPresentTags()
    {
        byte[] getTagsRequest = { (byte)MessageType.ListTags, 0x00, 0x07 };
        this.WriteSerialData(getTagsRequest);
        byte[] getTagsResponse = ReadSerialData();

        if (getTagsResponse[0] != (byte)MessageType.ListTags)
            throw new Exception("Unexpected response message type.");

        byte presentTagCount = getTagsResponse[4];

        List<Tag> tags = new List<Tag>();
        for (byte i = 0; i < presentTagCount; ++i)
        {
            int idIndex = 5 + (i * 9); // Skip the header of 5, tag length is 8 bytes + 1 bytes gap
            byte[] tagID = new byte[8];
            Array.Copy(getTagsResponse, idIndex + 1, tagID, 0, 8);
            tags.Add(new Tag(tagID));
        }

        return tags;
    }

    private string getTagData(Tag tag)
    {
        byte[] readTagRequest = { (byte)MessageType.ReadTag,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x06
        };
        Array.Copy(tag.ID, 0, readTagRequest, 1, 8);
        WriteSerialData(readTagRequest);
        byte[] readTagResponse = ReadSerialData();

        if (readTagResponse[0] != (byte)MessageType.ReadTag)
            throw new Exception("Unexpected response message type.");

        byte[] returnedTagID = readTagRequest.Skip(1).Take(8).ToArray();
        if (!Enumerable.SequenceEqual(returnedTagID, tag.ID))
        {
            throw new Exception("Read ID mismatch.");
        }

        if (readTagResponse[1] != 0)
        {
            return string.Empty;
        }

        char[] data = readTagResponse
            .Skip(13)
            .TakeWhile(x => x != 3)
            .Where(x => x >= 48 && x <= 57)
            .Select(x => (char)x)
            .ToArray();

        return new string(data);
    }
    */
    }
}
