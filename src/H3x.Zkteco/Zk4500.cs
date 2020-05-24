using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Dynamic;

namespace H3x.Zkteco
{
    public class Zk4500 : IDisposable
    {
        private const ushort USB_VID = 0x1B55;

        private const ushort USB_PID = 0x0840;

        private readonly IUsbDevice _usbDevice;

        private Zk4500(IUsbDevice usbDevice)
        {
            _usbDevice = usbDevice;
        }

        public static Zk4500 CreateAndOpenDevice()
        {
            using (var context = new UsbContext())
            {
                // use the predicate based Find method, as using a UsbDeviceFinder does not work for me
                // tested on Windows 10 18363, using lib version 3.0.81-alpha
                var usbDevice = context.Find(e =>
                {
                    if (e.VendorId == USB_VID && e.ProductId == USB_PID)
                        return true;
                    else
                        return false;
                });

                // open
                usbDevice.Open();

                // configure
                usbDevice.SetConfiguration(1);
                if (!usbDevice.ClaimInterface(0))
                    throw new UsbException("Could not claim USB interface 0");

                return new Zk4500(usbDevice);
            }
        }

        public void SetGreenLedOn()
        {
            var setupPkt = new UsbSetupPacket(0x40, 225, 1, 0, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");
        }

        public void SetGreenLedOff()
        {
            var setupPkt = new UsbSetupPacket(0x40, 225, 0, 0, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");
        }

        public void SetRedLedOn()
        {
            var setupPkt = new UsbSetupPacket(0x40, 225, 1, 1, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");
        }

        public void SetRedLedOff()
        {
            var setupPkt = new UsbSetupPacket(0x40, 225, 0, 1, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");
        }

        public void SetBuzzerOn()
        {
            var setupPkt = new UsbSetupPacket(0x40, 225, 1, 2, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");
        }

        public void SetBuzzerOff()
        {
            var setupPkt = new UsbSetupPacket(0x40, 225, 0, 2, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");
        }

        private byte[] GetRawImageInternal(int usbTimeout)
        {
            var setupPkt = new UsbSetupPacket(0x40, 229, 0, 0, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");

            var epReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep02, 0);

            var buffer = new byte[307200];
            var usbError = epReader.Read(buffer, 0, 307200, usbTimeout, out int transferLength);
            if (usbError != LibUsbDotNet.Error.Success)
                throw new UsbException(usbError);
            else if (transferLength != 307200)
                throw new UsbException($"USB transfer failed, expected 307200 bytes but received {transferLength}");

            return buffer;
        }

        public byte[] GetRawImage(int usbTimeout = 1000)
        {
            // ignore the first image as the device always sends an old buffered image first
            // this makes sure we always get a 'live' image using this method
            GetRawImageInternal(usbTimeout);
            return GetRawImageInternal(usbTimeout);
        }

        #region IDisposable implementation

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _usbDevice.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
