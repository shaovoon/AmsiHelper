using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanContentCSharpNet6
{
    public class AmsiHelper : IDisposable
    {
        private bool disposedValue;
        private IntPtr Context = IntPtr.Zero;
        private IntPtr Session = IntPtr.Zero;

        private bool ValidContext = false;
        private bool ValidSession = false;

        public AmsiHelper(string appName)
        {
            uint hr = AmsiMethods.AmsiInitialize(appName, out Context);
            ValidContext = (hr == 0);

            if (ValidContext)
            {
                hr = AmsiMethods.AmsiOpenSession(Context, out Session);
                ValidSession = (hr == 0);
            }
        }

        private void Destroy()
        {
            if (ValidContext && ValidSession)
                AmsiMethods.AmsiCloseSession(Context, Session);

            if (ValidContext)
                AmsiMethods.AmsiUninitialize(Context);

            ValidSession = false;
            ValidContext = false;
        }
        public bool ScanString(string text, string contentName, out bool isMalware)
        {
            isMalware = false;

            if (!ValidContext || !ValidSession)
                return false;

            uint result;
            uint hr = AmsiMethods.AmsiScanString(Context, text, contentName, Session, out result);
            if (hr == 0)
            {
                isMalware = IsMalware(result);
                return true;
            }
            return false;
        }
        public bool ScanBuffer(IntPtr buffer, uint length, string contentName, out bool isMalware)
        {
            isMalware = false;

            if (!ValidContext || !ValidSession)
                return false;

            uint result;
            uint hr = AmsiMethods.AmsiScanBuffer(Context, buffer, length, contentName, Session, out result);
            if (hr == 0)
            {
                isMalware = IsMalware(result);
                return true;
            }
            return false;
        }
        public bool NotifyOperation(IntPtr buffer, uint length, string contentName, out bool isMalware)
        {
            isMalware = false;

            if (!ValidContext)
                return false;

            uint result;
            uint hr = AmsiMethods.AmsiNotifyOperation(Context, buffer, length, contentName, out result);
            if (hr == 0)
            {
                isMalware = IsMalware(result);
                return true;
            }
            return false;
        }
        public bool IsValidAmsi()
        {
            return (ValidContext && ValidSession);
        }
        private static bool IsMalware(uint result)
        {
            return (result >= 32768);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                Destroy();
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~AmsiHelper()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
