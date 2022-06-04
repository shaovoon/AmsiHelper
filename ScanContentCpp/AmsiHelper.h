// The MIT License (MIT)
// Windows AntiMalware Scan Interface Helper 1.0.0
// Copyright (C) 2022 by Shao Voon Wong (shaovoon@yahoo.com)
//
// http://opensource.org/licenses/MIT

#pragma once
#include <amsi.h>

#pragma comment(lib, "Amsi")

/*
// Public interface of AmsiHelper
class AmsiHelper
{
	AmsiHelper();
	~AmsiHelper();

	bool IsValidAmsi() const;

	bool ScanString(LPCWSTR text, LPCWSTR contentName, bool* isMalware);
	bool ScanBuffer(PVOID buffer, ULONG length, LPCWSTR contentName, bool* isMalware);
	bool NotifyOperation(PVOID buffer, ULONG length, LPCWSTR contentName, bool* isMalware);
};
*/

class AmsiHelper
{
public:
	AmsiHelper(LPCWSTR appName)
		: Context(0)
		, Session(0)
		, ValidContext(false)
		, ValidSession(false)
	{
        HRESULT hr = AmsiInitialize(appName, &Context);
        ValidContext = (hr == S_OK);

        if (ValidContext)
        {
            hr = AmsiOpenSession(Context, &Session);
			ValidSession = (hr == S_OK);
		}
	}
	~AmsiHelper()
	{
		if (ValidContext && ValidSession)
			AmsiCloseSession(Context, Session);

		if (ValidContext)
			AmsiUninitialize(Context);

		ValidSession = false;
		ValidContext = false;
	}

	bool ScanString(LPCWSTR text, LPCWSTR contentName, bool* isMalware)
	{
		*isMalware = false;

		if (!ValidContext || !ValidSession)
			return false;

		AMSI_RESULT result;
		HRESULT hr = AmsiScanString(Context, text, contentName, Session, &result);
		if (hr == S_OK)
		{
			*isMalware = IsMalware(result);
			return true;
		}
		return false;
	}
	bool ScanBuffer(PVOID buffer, ULONG length, LPCWSTR contentName, bool* isMalware)
	{
		*isMalware = false;

		if (!ValidContext || !ValidSession)
			return false;

		AMSI_RESULT result;
		HRESULT hr = AmsiScanBuffer(Context, buffer, length, contentName, Session, &result);
		if (hr == S_OK)
		{
			*isMalware = IsMalware(result);
			return true;
		}
		return false;
	}
	bool NotifyOperation(PVOID buffer, ULONG length, LPCWSTR contentName, bool* isMalware)
	{
		*isMalware = false;

		if (!ValidContext)
			return false;

		AMSI_RESULT result;
		HRESULT hr = AmsiNotifyOperation(Context, buffer, length, contentName, &result);
		if (hr == S_OK)
		{
			*isMalware = IsMalware(result);
			return true;
		}
		return false;
	}
	bool IsValidAmsi() const
	{
		return (ValidContext && ValidSession);
	}
private:
	static bool IsMalware(AMSI_RESULT result)
	{
		return (result >= AMSI_RESULT_DETECTED);
	}

	HAMSICONTEXT Context;
	HAMSISESSION Session;

	bool ValidContext;
	bool ValidSession;


};