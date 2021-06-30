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

        public ushort CalcCRC16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            crc ^= 0xFFFF;
            return crc;
        }


        public Program()
        {
            byte[] test = { 0x00, 0x05, 0xFE, 0x00, 0x07 };
            Console.WriteLine(CalcCRC16(test));

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
            byte[] getTagsRequest = { (byte)MessageType.ListTags, 0x00, 0x07 };
            sendData(getTagsRequest);
            byte[] getTagsResponse = readData();

            if (getTagsResponse[0] != (byte)MessageType.ListTags)
                throw new Exception("Unexpected response message type.");

            byte presentTagCount = getTagsResponse[4];

            List<Tag> tags = new List<Tag>();
            for(byte i = 0; i < presentTagCount; ++i)
            {
                int idIndex = 6 + (i * 10); // Skip the header of 5, tag length is 8 bytes + 2 bytes tail
                byte[] tagID = new byte[8];
                Array.Copy(getTagsResponse, idIndex, tagID, 0, 8);
                tags.Add(new Tag(tagID));
            }
            Console.WriteLine(getTagsResponse.ToReadable());

            return tags;
        }

        /// <summary>
        /// Sends data to the reader over serial.
        /// Calculates payload header and CRC.
        /// </summary>
        /// <param name="data">Data to send to the reader.</param>
        private void sendData(byte[] data)
        {
            if (data.Length > 253)
                throw new ArgumentException("Data length must be less than 256");

            byte[] fullPayload = new byte[data.Length + 3 + 2];
            fullPayload[0] = 0xD6;              // Not sure what this means at this stage, looks like setup (D5) vs run (D6) 
            fullPayload[1] = 0x00;              // ¯\_(ツ)_/¯    Larger data length? Probably
            fullPayload[2] = (byte)(data.Length + 2); // Length of the payload data
            Array.Copy(data, 0, fullPayload, 3, data.Length);

            // Calculate CRC from payload
            byte[] crcData = new byte[fullPayload.Length - 3];
            Array.Copy(fullPayload, 1, crcData, 0, crcData.Length);
            ushort crc = CalcCRC16(crcData);
            Console.WriteLine(crc.ToString("X"));

            // Insert CRC into last two bytes of payload
            fullPayload[fullPayload.Length - 2] = (byte)(crc >> 8);
            fullPayload[fullPayload.Length - 1] = (byte)(crc & 0x00FF);

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
