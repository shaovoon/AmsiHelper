// See https://aka.ms/new-console-template for more information
using ScanContentCSharpNet6;

/*
// Example of using the raw AMSI functions
IntPtr amsiContext = IntPtr.Zero;
uint hr = AmsiMethods.AmsiInitialize("ScanContentCSharp", out amsiContext);
if (hr != 0)
{
    Console.WriteLine("AmsiInitialize failed!");
    return;
}
IntPtr amsiSession = IntPtr.Zero;
hr = AmsiMethods.AmsiOpenSession(amsiContext, out amsiSession);
if (hr != 0)
{
    Console.WriteLine("AmsiOpenSession failed!");
    AmsiMethods.AmsiUninitialize(amsiContext);
    return;
}

uint result = 0;
hr = AmsiMethods.AmsiScanString(amsiContext, "Hello World!", "Testing.txt", amsiSession, out result);
if (hr != 0)
{
    Console.WriteLine("AmsiScanString failed!");
}
else
{
    if (AmsiMethods.IsMalware(result))
        Console.WriteLine("Malware detected");
    else
        Console.WriteLine("No malware detected");
}

AmsiMethods.AmsiCloseSession(amsiContext, amsiSession);

AmsiMethods.AmsiUninitialize(amsiContext);
*/

// Example of using the AMSI wrapper class: AmsiHelper
using (AmsiHelper amsi = new AmsiHelper("ScanContentCSharpNet6"))
{
    if (!amsi.IsValidAmsi())
        Console.WriteLine("AmsiOpenSession failed!");

    bool isMalware = false;
    if (!amsi.ScanString("Hello World!", "Testing.txt", out isMalware))
        Console.WriteLine("AmsiScanString failed!");
    else
    {
        if (isMalware)
            Console.WriteLine("Malware detected");
        else
            Console.WriteLine("No malware detected");
    }
}
