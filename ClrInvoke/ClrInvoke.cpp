// ClrInvoke.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include "pch.h"
#include <iostream>
#include "CLRHost.h"
#include "Types.h"

extern "C" void _declspec(dllexport)  CMethod() {
	printf_s("CMethod called\n");
}
typedef void(_stdcall *SharpCallback)(void* data, unsigned int dwSize);

extern "C" void _declspec(dllexport) _stdcall  SetCallback(SharpCallback callback) {
	auto info = "SetCallback called\n";
	auto msgSize = sizeof(Message) + strlen(info);

	auto msg  = (Message*)malloc(msgSize);
	msg->Length = msgSize;
	msg->Type = 0x1;
	memcpy(msg + 1,info, strlen(info));

	callback((void*)msg, msgSize);
	free(msg);
}

int main(int argc, CHAR* argv[])
{
	auto Host = new CLRHost();
	Host->Start();
	if (Host->IsOK()) {
		auto dllPath = TEXT("DotNet.dll");
		unsigned long result;
		Host->LoadIntoDefaultAppDomain(dllPath, TEXT("DotNet.ClrClass"), TEXT("ClrExcute"), NULL, result);

		if (!Host->IsOK()) {
			auto errorCode = Host->GetErrorResult();
			printf_s("Failed to start CLR and load DotNet.dll (%d)\n", errorCode);
		}
	}
}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门提示: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
