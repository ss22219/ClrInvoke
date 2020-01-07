#pragma once
#include <mscoree.h>

class CLRHost
{
public:
	CLRHost();

	bool Start();

	long GetErrorResult() const {
		return errorResult;
	}

	bool IsOK() const {
		return errorResult >= 0;
	}

	bool LoadIntoDefaultAppDomain(const wchar_t * assemblyPath, const wchar_t * className, const wchar_t * staticMethodName, const wchar_t * stringArgument, unsigned long & result);
	~CLRHost();
private:
		struct ICLRMetaHost *MetaHost;
		struct ICLRRuntimeInfo *RuntimeInfo;
		struct ICLRRuntimeHost *ClrRuntimeHost;
		long errorResult;
		char PassPointer[64];
};

