using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WindowsApi
{
    public static class ProcessAPI
    {
        public static List<ProcessModule> GetProcessModule(int processId)
        {
            var handle = CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.Module32, (uint)processId);
            if (handle == IntPtr.Zero || (uint)handle == 0xffffffff)
                return new List<ProcessModule>();
            var result = new List<ProcessModule>();
            var moduleEntry = new MODULEENTRY32();
            moduleEntry.dwSize = (uint)Marshal.SizeOf(moduleEntry);
            if (Module32First(handle, ref moduleEntry))
            {
                do
                {
                    result.Add(new ProcessModule()
                    {
                        FileName = moduleEntry.szExePath,
                        ModuleSize = moduleEntry.modBaseSize,
                        BaseAddress = moduleEntry.modBaseAddr,
                        ModuleName = moduleEntry.szModule
                    });
                } while (Module32Next(handle, ref moduleEntry));
            }
            return result;
        }

        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            All = (HeapList | Process | Thread | Module),
            Inherit = 0x80000000,
            NoHeaps = 0x40000000
        }


        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct MODULEENTRY32
        {
            internal uint dwSize;
            internal uint th32ModuleID;
            internal uint th32ProcessID;
            internal uint GlblcntUsage;
            internal uint ProccntUsage;
            internal IntPtr modBaseAddr;
            internal uint modBaseSize;
            internal IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExePath;
        }


        public enum ChangeWindowMessageFilterFlags : uint
        {
            Add = 1,
            Remove = 2
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, uint th32ProcessID);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        public static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
         ProcessAccessFlags processAccess,
         bool bInheritHandle,
         int processId);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("user32.dll", EntryPoint = "ChangeWindowMessageFilter")]
        public static extern bool ChangeWindowMessageFilter(uint message, ChangeWindowMessageFilterFlags dwFlag);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        public static extern IntPtr LoadLibrary( string lpLibFileName);
    }

    public class ProcessModule
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 模块大小
        /// </summary>
        public uint ModuleSize { get; set; }
        /// <summary>
        /// 内存地址
        /// </summary>
        public IntPtr BaseAddress { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; internal set; }
    }
}
