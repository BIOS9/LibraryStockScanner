// -----------------------------------------------------------------------
// <copyright file="SerialReaderChannel.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication
{
    using System;
    using System.IO.Ports;
    using System.Linq;

    /// <summary>
    /// A reader channel that uses a serial port for
    /// the underlying communication.
    /// </summary>
    internal class SerialReaderChannel : IReaderChannel, IDisposable
    {
        private const int SerialBaudRate = 19200;
        private const Parity SerialParity = Parity.None;
        private const int SerialDataBits = 8;
        private const StopBits SerialStopBits = StopBits.One;

        private readonly SerialPort serialPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialReaderChannel"/> class.
        /// </summary>
        /// <param name="comPort">Serial Communications Port that is connected to the reader.</param>
        public SerialReaderChannel(string comPort)
        {
            this.serialPort = new SerialPort(
                comPort,
                SerialBaudRate,
                SerialParity,
                SerialDataBits,
                SerialStopBits);
        }

        /// <inheritdoc/>
        public bool IsReady => throw new NotImplementedException();

        /// <inheritdoc/>
        public ReaderResponse Tranceive(ReaderCommand command)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.serialPort.Dispose();
        }
    }
}
