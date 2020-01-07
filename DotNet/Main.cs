using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using WindowsApi;

namespace DotNet
{
    public class ClrClass
    {
        private static Message message;
        private static string callbackStr;


        [DllImport("ClrInvoke.exe")]
        private extern static void CMethod();
        [DllImport("ClrInvoke.exe", CallingConvention = CallingConvention.StdCall)]
        private extern static void SetCallback(CCallback callback);

        [DllImport("ClrLoader.dll", EntryPoint = "CMethod")]
        private extern static void CMethod2();

        public static int ClrExcute(string paramstr)
        {
            try
            {
                //加载DLL
                LoadDlls();
                if (ProcessAPI.GetProcessModule(Process.GetCurrentProcess().Id).Any(m => m.ModuleName ==  "ClrLoader.dll"))
                    CMethod2();
                else
                {
                    //c# 调用c++函数
                    CMethod();

                    //c++调用c#函数
                    SetCallback(_cCallback);
                }
                JsonConvert.DeserializeObject("{}");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 1;
        }

        private static void LoadDlls()
        {
            var dllDir = Path.GetDirectoryName(typeof(ClrClass).Assembly.Location);
            var dlls = Directory.GetFiles(dllDir, "*.dll");
            foreach (var dll in dlls)
            {
                try
                {
                    Assembly.Load(File.ReadAllBytes(dll));
                }
                catch (Exception)
                {
                }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private delegate void CCallback(IntPtr buff, uint dwSize);
        //使用字段存放，防止GC回收函数
        private static readonly CCallback _cCallback = (ptr, len) =>
        {
            message = Marshal.PtrToStructure<Message>(ptr);
            Console.WriteLine($"callback: type:{message.Type} length:{message.Length}");
            if (message.Type == 1)
                callbackStr = Marshal.PtrToStringAnsi(ptr + 8, (int)(message.Length - 8));
        };
    }
}