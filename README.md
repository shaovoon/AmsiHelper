# Bring Antimalware Scanning Into Your .NET 6 Application

## Table of Contents

* Introduction
* Raw Win32 Functions
* Raw Win32 Functions Example
* Wrapper Class: AmsiHelper
* Wrapper Class Example
* History

## Introduction

For those of you who came in and half-expecting a long and complicated article, things couldn&#39;t be simpler&nbsp;through&nbsp;[Antimalware Scan Interface (AMSI)](https://docs.microsoft.com/en-us/windows/win32/amsi/antimalware-scan-interface-portal) available on Windows 10 since 2015 to allow the anti-malware scanning&nbsp;on user-supplied content. AMSI is agnostic of the antimalware vendor. The scanning&nbsp;is&nbsp;done through&nbsp;the&nbsp;anti-malware installed on the user&#39;s computer. Anti-malware vendor and version are not provided by AMSI even though this information is vital for contacting the anti-malware vendor to report false positive detection. Another missing information is the name of malware detected and the type of vulnerability to aid the further investigation. AMSI is&nbsp;actively used in Microsoft products such as MS Office.

This article is divided into two main sections: the raw API section and the wrapper class section. Another way to call AMSI is through its COM API but the complexity of COM interop on .NET is best avoided since&nbsp;the raw API provides the same functionality as the COM API. Only raw Win32 AMSI APIs are&nbsp;covered here.

## Raw Win32 Functions

### AMSI Context

__Note__: The function definitions listed in this section have been converted through P/Invoke. For instance, the `HRESULT` return type has been changed to `uint` and the handle types for the context and session are changed to `IntPtr`. To use AMSI, a context must be first created with `AmsiInitialize()`. Supply&nbsp;your application name in&nbsp;`appName&nbsp;`parameter. The function returns&nbsp;a `HRESULT `where&nbsp;`S_OK&nbsp;`(0) means success. An AMSI context may&nbsp;fail&nbsp;to initialize when&nbsp;Microsoft Defender is disabled and no other anti-malware product&nbsp;is present on the user&#39;s computer.

```CSharp
uint AmsiInitialize(String appName, out IntPtr amsiContext);
```

```Cpp
HRESULT AmsiInitialize(LPCWSTR appName, HAMSICONTEXT *amsiContext);
```

After use, the context must be uninitialized with a call to&nbsp;`AmsiUninitialize()`.

```CSharp
void AmsiUninitialize(IntPtr amsiContext);
```

```Cpp
void AmsiUninitialize(HAMSICONTEXT amsiContext);
```

### AMSI Session

To do scanning, if you do not want to group the scannings under a session or when you only have one content to scan, a session is not required. To open a session, call&nbsp;`AmsiOpenSession()` with the context created from&nbsp;`AmsiInitialize().`

```CSharp
uint AmsiOpenSession(IntPtr amsiContext, out IntPtr amsiSession);
```

```Cpp
HRESULT AmsiOpenSession(HAMSICONTEXT amsiContext, HAMSISESSION *amsiSession);
```

After the scanning is done, close the session with a call to `AmsiCloseSession()`&nbsp;with the same&nbsp;context supplied to `AmsiOpenSession()`.

```CSharp
void AmsiCloseSession(IntPtr amsiContext, IntPtr amsiSession);
```

```Cpp
void AmsiCloseSession(HAMSICONTEXT amsiContext, HAMSISESSION amsiSession);
```

### AMSI Scan Functions

There are two types of scanning: string and binary buffer. Use `AmsiScanString()`&nbsp;to scan text and the `AmsiScanBuffer()` to scan buffer. `contentName` is either the filename or the URL where this content is from. `amsiSession` can be `IntPtr.Zero`&nbsp;for&nbsp;not providing a&nbsp;session. `result` is the output of these functions. `IsMalware()`&nbsp;must be called on `result` to check&nbsp;if it indicates&nbsp;malware. Other parameters are self-explanatory. The function returns&nbsp;a `HRESULT `where&nbsp;`S_OK&nbsp;`(0) means success. `AmsiScanString()` is&nbsp;called on the&nbsp;script written in Powershell, JavaScript, VBScript&nbsp;or&nbsp;Office VBA macros.

```CSharp
uint AmsiScanString(IntPtr amsiContext, String text, String contentName,
            IntPtr amsiSession, out uint result);
```

```Cpp
HRESULT AmsiScanString(HAMSICONTEXT amsiContext, LPCWSTR string, 
            LPCWSTR contentName, HAMSISESSION amsiSession, 
            AMSI_RESULT *result);
```

`AmsiScanBuffer()` is for scanning a binary buffer that could&nbsp;contain a malicious executable program. An example scenario is&nbsp;your software implements a plugin system where any third party can supply their own plugins to extend your software, it is advisable to call `AmsiScanBuffer()` to&nbsp;scan the DLLs prior to loading and running them.

```CSharp
uint AmsiScanBuffer(IntPtr amsiContext, IntPtr buffer, uint length, String contentName,
            IntPtr amsiSession, out uint result);
```

```Cpp
HRESULT AmsiScanBuffer(HAMSICONTEXT amsiContext, PVOID buffer, ULONG length, 
            LPCWSTR contentName, HAMSISESSION amsiSession, 
            AMSI_RESULT *result);
```

`AmsiNotifyOperation()` is to notify the anti-malware in the case that malware is found. No session is required to call this function. It is best not to assume scanning is done in `AmsiNotifyOperation` because this function may not be implemented by third-party anti-malware vendors, so do not rely on the `result`.

```CSharp
uint AmsiNotifyOperation(IntPtr amsiContext, IntPtr buffer, 
                         uint length, String contentName, out uint result);
```

```Cpp
HRESULT AmsiNotifyOperation(HAMSICONTEXT amsiContext, PVOID buffer, ULONG length,
                            LPCWSTR contentName, AMSI_RESULT  *result);
```

`IsMalware`&nbsp;is defined as&nbsp;below:

```CSharp
bool IsMalware(uint result)
{
    return (result >= 32768);
}
```

```Cpp
bool IsMalware(AMSI_RESULT result)
{
    return (result >= AMSI_RESULT_DETECTED);
}
```

## Raw Win32 Functions Example

Below is an example of using the raw Win32 AMSI functions.

```CSharp
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
hr = AmsiMethods.AmsiScanString
     (amsiContext, "Hello World!", "Testing.txt", amsiSession, out result);
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
```

```Cpp
// Example of using the raw AMSI functions
HAMSICONTEXT amsiContext;
HRESULT hr = AmsiInitialize(L"ScanContentCpp", &amsiContext);
if (FAILED(hr))
{
    printf("AmsiInitialize failed!");
    return 1;
}

HAMSISESSION amsiSession;
hr = AmsiOpenSession(amsiContext, &amsiSession);
if (FAILED(hr))
{
    printf("AmsiOpenSession failed!");
    AmsiUninitialize(amsiContext);
    return 1;
}

AMSI_RESULT result;
hr = AmsiScanString(amsiContext, L"Hello World!", L"Testing.txt", amsiSession, &result);
if (FAILED(hr))
    printf("AmsiScanString failed!");
else
{
    if (result >= AMSI_RESULT_DETECTED)
        printf("Malware detected");
    else
        printf("No malware detected");
}

AmsiCloseSession(amsiContext, amsiSession);

AmsiUninitialize(amsiContext);
```

## Wrapper Class: AmsiHelper

A wrapper called `AmsiHelper` is written to simplify the AMSI usage. A context and session are created in the constructor and destroyed in the&nbsp;finalizer. Its `public` methods are listed below:

```CSharp
AmsiHelper(string appName);  // Constructor
~AmsiHelper(string appName); // Finalizer

bool IsValidAmsi();          // Check if managed to get a ASMI context and session

bool ScanString(string text, string contentName, out bool isMalware); // Scan text
bool ScanBuffer(IntPtr buffer, uint length, 
                string contentName, out bool isMalware);              // Scan buffer
bool NotifyOperation(IntPtr buffer, uint length, string contentName, 
                     out bool isMalware); // Notify anti-malware of this buffer
```

```Cpp
AmsiHelper();  // Constructor
~AmsiHelper(); // Destructor

bool IsValidAmsi() const; // Check if managed to get a ASMI context and session

bool ScanString(LPCWSTR text, LPCWSTR contentName, bool* isMalware); // Scan text
bool ScanBuffer(PVOID buffer, ULONG length, 
                LPCWSTR contentName, bool* isMalware); // Scan buffer
bool NotifyOperation(PVOID buffer, ULONG length, LPCWSTR contentName,
                     bool* isMalware); // Notify anti-malware of this buffer
```

## Wrapper Class Example

Below is an example of using the `AmsiHelper`.

```CSharp
// Example of using the AMSI wrapper class: AmsiHelper
using (AmsiHelper amsi = new AmsiHelper("ScanContentCSharp"))
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
```

```Cpp
// Example of using the AMSI wrapper class: AmsiHelper
AmsiHelper amsi(L"ScanContentCpp");

if (!amsi.IsValidAmsi())
    printf("AmsiOpenSession failed!");

bool isMalware = false;
if (!amsi.ScanString(L"Hello World!", L"Testing.txt", &isMalware))
    printf("AmsiScanString failed!");
else
{
    if (isMalware)
        printf("Malware detected");
    else
        printf("No malware detected");
}
```

Though the `AmsiHelper` simplifies the usage and resource management, the raw APIs that allow the flexibility of not using the session. .NET Framework, .NET 6 and C++ sample codes are made available for download.

## History

* __8<sup>th</sup> June, 2022__: Added C++ source code examples to the article via C++ tab
* __3<sup>rd</sup> May, 2022__: First release

## Other Articles in the _Bring Your..._ Series

* [Bring Your C++ Code to the Web](https://www.codeproject.com/Articles/5160967/Bring-Your-Cplusplus-Code-to-the-Web)
* [Bring C++ Graphics to the Web](https://www.codeproject.com/Articles/5163290/Bring-Cplusplus-Graphics-to-the-Web)
* [Bring Your Animations to H264/HEVC Video](https://www.codeproject.com/Articles/5161187/Bring-Your-Animations-to-H264-HEVC-Video)
* [Bring Your Existing Application to Microsoft Store](https://www.codeproject.com/Articles/5162884/Bring-Your-Existing-Application-to-Microsoft-Store)
* [Bring Your C++ OpenGL Code to the Web](https://www.codeproject.com/Articles/5165465/Bring-Your-Cplusplus-OpenGL-Code-to-the-Web)
