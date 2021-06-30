using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibHax
{
    class Program
    {
        SerialPort _serial;

        private enum MessageType : byte
        {
            ListTags = 0xFE,
            ReadTag = 0x02,
        }

        private class Tag
        {
            public readonly byte[] ID;

            public Tag(byte[] iD)
            {
                ID = iD ?? throw new ArgumentNullException(nameof(iD));
            }

            public override string ToString()
            {
                return ID.ToReadable();
            }
        }

        public Program()
        {
            _serial = new SerialPort("COM6", 19200, Parity.None, 8, StopBits.One);
            _serial.Open();
            Console.WriteLine("Opened");

            while (true)
            {
                List<Tag> tags = getPresentTags();
                Console.WriteLine($"Found {tags.Count} tags.");
                foreach (Tag tag in tags)
                {
                    Console.WriteLine(tag);
                }
                Console.WriteLine();
            }
        }

        private List<Tag> getPresentTags()
        {
            byte[] getTagsRequest = { (byte)MessageType.ListTags, 0x00, 0x07, 0xDA, 0x02 };
            sendData(getTagsRequest);
            byte[] getTagsResponse = readData();

            if (getTagsResponse[0] != (byte)MessageType.ListTags)
                throw new Exception("Unexpected response message type.");

            byte presentTagCount = getTagsResponse[4];

            List<Tag> tags = new List<Tag>();
            for(byte i = 0; i < presentTagCount; ++i)
            {
                int idIndex = 5 + (i * 10); // Skip the header of 5, tag length is 8 bytes + 2 bytes tail
                byte[] tagID = new byte[8];
                Array.Copy(getTagsResponse, idIndex, tagID, 0, 8);
                tags.Add(new Tag(tagID));
            }
            //Console.WriteLine(getTagsResponse.ToReadable());

            return tags;
        }

        /// <summary>
        /// Sends data to the reader over serial.
        /// Calculates payload header.
        /// </summary>
        /// <param name="data">Data to send to the reader.</param>
        private void sendData(byte[] data)
        {
            if (data.Length > 255)
                throw new ArgumentException("Data length must be less than 256");

            byte[] fullPayload = new byte[data.Length + 3];
            fullPayload[0] = 0xD6;              // Not sure what this means at this stage, looks like setup (D5) vs run (D6) 
            fullPayload[1] = 0x00;              // ¯\_(ツ)_/¯    Larger data length? Probably
            fullPayload[2] = (byte)data.Length; // Length of the payload data
            Array.Copy(data, 0, fullPayload, 3, data.Length);

            _serial.BaseStream.Write(fullPayload, 0, fullPayload.Length);
        }

        /// <summary>
        /// Reads data from the reader over serial.
        /// </summary>
        private byte[] readData()
        {
            byte[] header = new byte[3];
            int readCount = 0;
            while ((readCount += _serial.Read(header, readCount, header.Length - readCount)) < header.Length);
            
            if(header[0] != 0xD6)
            {
                throw new DataMisalignedException();
            }

            byte dataLength = header[2];
            byte[] data = new byte[dataLength];
            readCount = 0;
            while ((readCount += _serial.Read(data, readCount, data.Length - readCount)) < data.Length);

            return data;
        }

        static void Main(string[] args)
        {
            new Program();
            Console.ReadLine();
        }
    }
}
