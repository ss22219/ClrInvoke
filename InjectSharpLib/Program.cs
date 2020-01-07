using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WindowsApi;

namespace InjectSharpLib
{
    class Program
    {
        const string InjectNativeDll = "ClrLoader.dll";
        const string InjectSharpDll = "DotNet.dll";
        static void Main(string[] args)
        {
            var strPid = args.Length > 0 ? args[0] : null;
            if (int.TryParse(strPid, out int pid))
            {
                RemoteExcuteAPI.InjectDLL(pid, InjectNativeDll);
                ProcessAPI.LoadLibrary(InjectNativeDll);
                var module = ProcessAPI.GetProcessModule(Process.GetCurrentProcess().Id).First(m=>m.ModuleName == InjectNativeDll);
                var startProc = ProcessAPI.GetProcAddress(module.BaseAddress, "Start") - (int)module.BaseAddress;
                var remotModule = ProcessAPI.GetProcessModule(pid).First(m => m.ModuleName == InjectNativeDll);
                RemoteExcuteAPI.ExcuteRemoteFunction(pid, remotModule.BaseAddress + (int)startProc, Encoding.Unicode.GetBytes(Directory.GetCurrentDirectory() + "\\" + InjectSharpDll));
            }
        }
    }
}
