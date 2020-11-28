using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Threading.Tasks;
using Ultz.Dispatcher;

namespace H3x.Zkteco
{
    public class Zk4500 : IDisposable
    {
        private const ushort USB_VID = 0x1B55;

        private const ushort USB_PID = 0x0840;

        private readonly IUsbDevice _usbDevice;

        private readonly Dispatcher _dispatcher;

        private readonly UsbContext _context;

        private Zk4500(Dispatcher dispatcher, UsbContext context, IUsbDevice usbDevice)
        {
            _usbDevice = usbDevice;
            _dispatcher = dispatcher;
            _context = context;
        }

        #region Create device, lowlevel USB helper functions

        public static Zk4500 CreateAndOpenDevice(Func<string, bool> predicate = null)
        {
            var dispatcher = new Dispatcher();

            return dispatcher.Invoke(() =>
            {
                return CreateAndOpenDeviceInternal(dispatcher, predicate);
            });
        }

        public static async Task<Zk4500> CreateAndOpenDeviceAsync(Func<string, bool> predicate = null)
        {
            var dispatcher = new Dispatcher();

            return await dispatcher.InvokeAsync(() =>
            {
                return CreateAndOpenDeviceInternal(dispatcher, predicate);
            });
        }

        private static Zk4500 CreateAndOpenDeviceInternal(Dispatcher dispatcher, Func<string, bool> predicate = null)
        {
            // use the predicate based Find method, as using a UsbDeviceFinder does not work for me
            // tested on Windows 10 18363, using lib version 3.0.81-alpha
            var usbContext = new UsbContext();
            var usbDevice = usbContext.Find(d =>
            {
                var isZk4500 = d.VendorId == USB_VID && d.ProductId == USB_PID;
                if (isZk4500 && predicate != null)
                    return predicate(d.Info.SerialNumber);
                return isZk4500;
            });
            if (usbDevice == null)
                return null;

            // open
            usbDevice.Open();

            // configure
            usbDevice.SetConfiguration(1);
            if (!usbDevice.ClaimInterface(0))
                throw new UsbException("Could not claim USB interface 0");

            return new Zk4500(dispatcher, usbContext, usbDevice);
        }

        private void ControlTransferInternal(byte bRequest, int wValue, int wIndex)
        {
            var setupPkt = new UsbSetupPacket(0x40, bRequest, wValue, wIndex, 0);
            var retVal = _usbDevice.ControlTransfer(setupPkt, Array.Empty<byte>(), 0, 0);
            if (retVal != 0)
                throw new UsbException($"ControlTransfer returned non zero value {retVal}");
        }

        public string SerialNumber => _dispatcher.Invoke(() => _usbDevice.Info.SerialNumber);

        #endregion

        #region LEDs, buzzer

        public void ResetDevice()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                _usbDevice.ResetDevice();
            });
        }

        public async Task ResetDeviceAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                _usbDevice.ResetDevice();
            });
        }

        public void SetGreenLedOn()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 1, 0);
            });
        }

        public async Task SetGreenLedOnAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 1, 0);
            });
        }

        public void SetGreenLedOff()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 0, 0);
            });
        }

        public async Task SetGreenLedOffAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 0, 0);
            });
        }

        public void SetRedLedOn()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 1, 1);
            });
        }

        public async Task SetRedLedOnAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 1, 1);
            });
        }

        public void SetRedLedOff()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 0, 1);
            });
        }

        public async Task SetRedLedOffAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 0, 1);
            });
        }

        public void SetBuzzerOn()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 1, 2);
            });
        }

        public async Task SetBuzzerOnAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 1, 2);
            });
        }

        public void SetBuzzerOff()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 0, 2);
            });
        }

        public async Task SetBuzzerOffAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 0, 2);
            });
        }

        public void SetWhiteLedOn()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 1, 3);
            });
        }

        public async Task SetWhiteLedOnAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 1, 3);
            });
        }

        public void SetWhiteLedOff()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            _dispatcher.Invoke(() =>
            {
                ControlTransferInternal(225, 0, 3);
            });
        }

        public async Task SetWhiteLedOffAsync()
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            await _dispatcher.InvokeAsync(() =>
            {
                ControlTransferInternal(225, 0, 3);
            });
        }

        #endregion

        #region Imaging

        public byte[] GetRawImage(int usbTimeout = 1000)
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            return _dispatcher.Invoke(() =>
            {
                // ignore the first image as the device always sends an old buffered image first
                // this makes sure we always get a 'live' image using this method
                GetRawImageInternal(usbTimeout);
                return GetRawImageInternal(usbTimeout);
            });
        }

        public async Task<byte[]> GetRawImageAsync(int usbTimeout = 1000)
        {
            if (disposing)
                throw new ObjectDisposedException(GetType().FullName);

            return await _dispatcher.InvokeAsync(() =>
            {
                // ignore the first image as the device always sends an old buffered image first
                // this makes sure we always get a 'live' image using this method
                GetRawImageInternal(usbTimeout);
                return GetRawImageInternal(usbTimeout);
            });
        }

        private byte[] GetRawImageInternal(int usbTimeout)
        {
            // request image
            ControlTransferInternal(229, 0, 0);

            // read image
            var epReader = _usbDevice.OpenEndpointReader(ReadEndpointID.Ep02, 0);

            var buffer = new byte[307200];
            var usbError = epReader.Read(buffer, 0, 307200, usbTimeout, out int transferLength);
            if (usbError != LibUsbDotNet.Error.Success)
                throw new UsbException(usbError);
            else if (transferLength != 307200)
                throw new UsbException($"USB transfer failed, expected 307200 bytes but received {transferLength}");

            return buffer;
        }

        #endregion

        #region IDisposable implementation

        private bool disposing, disposeValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposeValue)
            {
                if (disposing)
                {
                    this.disposing = true;
                    disposeValue = true;
                    using (_dispatcher)
                    {
                        _dispatcher.Invoke(() =>
                        {
                            _usbDevice.Dispose();
                            _context.Dispose();
                        });
                    }
                }
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
