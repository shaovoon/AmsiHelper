# AmsiHelper
Antimalware Scan Interface Helper for Windows 10/11

CSharp example of using the helper class

```CSharp
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
```

C++ example of using the helper class

```Cpp
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
