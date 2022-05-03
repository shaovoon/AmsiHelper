using System;
using System.Runtime.InteropServices;

namespace ScanContentCSharp
{
    public static class AmsiMethods
    {
        [DllImport("amsi.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint AmsiInitialize([MarshalAs(UnmanagedType.LPWStr)] String appName, out IntPtr amsiContext);

        [DllImport("amsi.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void AmsiUninitialize(IntPtr amsiContext);

        [DllImport("amsi.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint AmsiOpenSession(IntPtr amsiContext, out IntPtr amsiSession);

        [DllImport("amsi.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void AmsiCloseSession(IntPtr amsiContext, IntPtr amsiSession);

        [DllImport("amsi.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint AmsiScanBuffer(IntPtr amsiContext, IntPtr buffer, uint length, [MarshalAs(UnmanagedType.LPWStr)] String contentName,
            IntPtr amsiSession, out uint result);

        [DllImport("amsi.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint AmsiScanString(IntPtr amsiContext, [MarshalAs(UnmanagedType.LPWStr)] String text, [MarshalAs(UnmanagedType.LPWStr)] String contentName,
            IntPtr amsiSession, out uint result);

        [DllImport("amsi.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint AmsiNotifyOperation(IntPtr amsiContext, IntPtr buffer, uint length, [MarshalAs(UnmanagedType.LPWStr)] String contentName, out uint result);
        public static bool IsMalware(uint result)
        {
            return (result >= 32768);
        }
    }
}
