using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsApi
{
    public static class RemoteExcuteAPI
    {
        /// <summary>
        /// 注入模块到远程进程
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="modulePath"></param>
        /// <returns></returns>
        public static bool InjectDLL(int processId, string modulePath)
        {
            return ExcuteRemoteSystemFunction(processId, "kernel32.dll", "LoadLibraryA", Encoding.ASCII.GetBytes(modulePath));
        }

        /// <summary>
        /// 执行远程进程上的系统函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processId">进程id</param>
        /// <param name="moduleName">系统模块名称</param>
        /// <param name="functionName">函数名称</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExcuteRemoteSystemFunction<T>(int processId, string moduleName, string functionName, T param)
        {
            return ExcuteRemoteSystemFunction(processId, moduleName, functionName, StructToBytes(param, Marshal.SizeOf<T>()));
        }

        /// <summary>
        /// 将struct类型转换为byte[]
        /// </summary>
        public static byte[] StructToBytes(object structObj, int size)
        {
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try//struct_bytes转换
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in StructToBytes ! " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        /// <summary>
        /// 执行远程进程上的函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processId">进程id</param>
        /// <param name="lpFuncAddress">函数地址</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool ExcuteRemoteFunction<T>(int processId, IntPtr lpFuncAddress, T param)
        {
            return ExcuteRemoteFunction(processId, lpFuncAddress, StructToBytes(param, Marshal.SizeOf<T>()));
        }

        public static bool ExcuteRemoteFunction(int processId, IntPtr lpFuncAddress, byte[] param)
        {
            var hndProc = ProcessAPI.OpenProcess(
                                    ProcessAPI.ProcessAccessFlags.CreateThread | ProcessAPI.ProcessAccessFlags.VirtualMemoryOperation |
                                    ProcessAPI.ProcessAccessFlags.VirtualMemoryRead | ProcessAPI.ProcessAccessFlags.VirtualMemoryWrite
                                    | ProcessAPI.ProcessAccessFlags.QueryInformation
                                    , true, processId);
            if (hndProc == IntPtr.Zero)
                return false;

            var lpAddress = MemoryAPI.VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)param.Length, (0x1000 | 0x2000), 0X40);

            if (lpAddress == IntPtr.Zero)
            {
                ProcessAPI.CloseHandle(hndProc);
                return false;
            }

            if (MemoryAPI.WriteProcessMemory(hndProc, lpAddress, param, (uint)param.Length, 0) == 0)
            {
                ProcessAPI.CloseHandle(hndProc);
                return false;
            }

            if (ProcessAPI.CreateRemoteThread(hndProc, (IntPtr)null, IntPtr.Zero, lpFuncAddress, lpAddress, 0, (IntPtr)null) == IntPtr.Zero)
            {
                ProcessAPI.CloseHandle(hndProc);
                return false;
            }
            return true;
        }

        public static bool ExcuteRemoteSystemFunction(int processId, string moduleName, string functionName, byte[] param)
        {
            var hndProc = ProcessAPI.OpenProcess(
                ProcessAPI.ProcessAccessFlags.CreateThread | ProcessAPI.ProcessAccessFlags.VirtualMemoryOperation |
                ProcessAPI.ProcessAccessFlags.VirtualMemoryRead | ProcessAPI.ProcessAccessFlags.VirtualMemoryWrite
                | ProcessAPI.ProcessAccessFlags.QueryInformation
                , true, processId);
            if (hndProc == IntPtr.Zero)
                return false;

            //查找当前应用系统函数地址，本机上所有应用的系统函数地址都是相同的
            var lpFuncAddress = ProcessAPI.GetProcAddress(ProcessAPI.GetModuleHandle(moduleName), functionName);
            ProcessAPI.CloseHandle(hndProc);

            if (lpFuncAddress == IntPtr.Zero)
                return false;
            return ExcuteRemoteFunction(processId, lpFuncAddress, param);
        }
    }
}
