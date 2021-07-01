// -----------------------------------------------------------------------
// <copyright file="ReaderSerialTransceiver.cs" company="CIA">
// Copyright (c) CIA. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RfidAssetReader3M.ReaderCommunication.Transceivers
{
    using System;
    using System.IO.Ports;
    using System.Threading.Tasks;

    /// <summary>
    /// A reader channel that uses a serial port for
    /// the underlying communication.
    /// </summary>
    internal class ReaderSerialTransceiver : IReaderTransceiver, IDisposable, IAsyncDisposable
    {
        private const int SerialBaudRate = 19200;
        private const Parity SerialParity = Parity.None;
        private const int SerialDataBits = 8;
        private const StopBits SerialStopBits = StopBits.One;

        private readonly SerialPort serialPort;
        private ReaderStreamTransceiver tranceiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderSerialTransceiver"/> class.
        /// </summary>
        /// <param name="comPort">Serial Communications Port that is connected to the reader.</param>
        public ReaderSerialTransceiver(string comPort)
        {
            this.serialPort = new SerialPort(
                comPort,
                SerialBaudRate,
                SerialParity,
                SerialDataBits,
                SerialStopBits);
            this.tranceiver = new ReaderStreamTransceiver(this.serialPort.BaseStream);
        }

        /// <inheritdoc/>
        public bool IsReady => throw new NotImplementedException();

        /// <inheritdoc/>
        public ReaderResponse Transceive(ReaderCommand command)
        {
            return this.tranceiver.Transceive(command);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.serialPort.Dispose();
            this.tranceiver.Dispose();
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            // The serial port doesnt implement IAsyncDisposable :(
            return this.tranceiver.DisposeAsync();
        }
    }
}
