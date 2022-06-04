// The MIT License (MIT)
// Windows AntiMalware Scan Interface Helper 1.0.0
// Copyright (C) 2022 by Shao Voon Wong (shaovoon@yahoo.com)
//
// http://opensource.org/licenses/MIT

using System;
using System.Runtime.InteropServices;

namespace ScanContentCSharpNet6
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
