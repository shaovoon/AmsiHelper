// The MIT License (MIT)
// Windows AntiMalware Scan Interface Helper 1.0.0
// Copyright (C) 2022 by Shao Voon Wong (shaovoon@yahoo.com)
//
// http://opensource.org/licenses/MIT

#include <cstdio>
#include <amsi.h>
#include "AmsiHelper.h"
#pragma comment(lib, "Amsi")

/*
// Example of using the raw AMSI functions
int main()
{
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

    return 0;
}
*/

// Example of using the AMSI wrapper class: AmsiHelper
int main()
{
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

    return 0;
}

