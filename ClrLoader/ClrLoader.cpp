// ClrLoader.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"


#include <iostream>
#include "CLRHost.h"
#include "Types.h"

CLRHost* Host = NULL;
extern "C" void _declspec(dllexport)  Start(wchar_t* dllPath) {
	wprintf_s(TEXT("Loading SharpDll:%s\n"), dllPath);
	if (Host == NULL) {
		Host = new CLRHost();
		Host->Start();
		if (!Host->IsOK()) {
			printf_s("Failed to start CLR\n", Host->GetErrorResult());
			return;
		}

		unsigned long result;
		Host->LoadIntoDefaultAppDomain(dllPath, TEXT("DotNet.ClrClass"), TEXT("ClrExcute"), NULL, result);
		if (!Host->IsOK()) {
			printf_s("Failed to start CLR and load DotNet.dll (%d)\n", Host->GetErrorResult());
			return;
		}
	}
}

extern "C" void _declspec(dllexport)  CMethod() {
	printf_s("DotNet.dll injected!\n");
}
typedef void(_stdcall *SharpCallback)(void* data, unsigned int dwSize);

extern "C" void _declspec(dllexport) _stdcall  SetCallback(SharpCallback callback) {
	auto info = "SetCallback called\n";
	auto msgSize = sizeof(Message) + strlen(info);

	auto msg = (Message*)malloc(msgSize);
	msg->Length = msgSize;
	msg->Type = 0x1;
	memcpy(msg + 1, info, strlen(info));

	callback((void*)msg, msgSize);
	free(msg);
}