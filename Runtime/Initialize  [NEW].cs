using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using IL_Emulator_Dynamic;
using Runtime.OpCodes;

namespace ConversionBack
{
    public class Initialize
    {
        public static OpCode[] oneByteOpCodes;
        public static OpCode[] twoByteOpCodes;
        //public static StackTrace stackTrace;
        public static System.Reflection.Module callingModule;

        public static byte[] byteArrayResource;
        public static a Run;


        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", ExactSpelling = true)]
        private static extern IntPtr e(IntPtr intptr, string str);

        public delegate void a(byte[] bytes, int len, byte[] key, int keylen);

        public static void Initalize(object obj, string resName, string hehe, string binary)
        {
            
            resName = Str(resName);
            resName += hehe.Replace("d01lb3c=", "%#");
            
            //string text = obj.ToString();
            callingModule = new StackTrace().GetFrame(1).GetMethod().Module;
            byteArrayResource = extractResource(resName);
            Runtime.OpCodes.All.binr = new BinaryReader(new MemoryStream(extractResource(Str(binary)))); //Str(binary))
            Runtime.OpCodes.All.val = new ValueStack();
            Runtime.OpCodes.All.val.parameters = new object[1];
            All.val.parameters[0] = byteArrayResource;

            All.val.locals = new object[10];
            Runtime.OpCodes.All.run();
            IntPtr def;
            if (IntPtr.Size == 8)
            {
                ExtractEmbeddedDlls("wMeow0.dll", extractResource("MeoxD=+"));
                def = e(LoadDll("wMeow0.dll"), "a");
                
            }
            else
            {
                ExtractEmbeddedDlls("wMeow0.dll", extractResource("MeoxD=-"));
                def = e(LoadDll("wMeow0.dll"), "_a@16");
            }

            /*if (IntPtr.Size == 4)
            {
                ExtractEmbeddedDlls("wMeow0.dll", extractResource("MeoxD=-"));
                def = e(LoadDll("wMeow0.dll"), "_a@16");
            }
            else
            {
                ExtractEmbeddedDlls("wMeow0.dll", extractResource("MeoxD=+"));
                def = e(LoadDll("wMeow0.dll"), "a");
            }*/

            Run = (a)Marshal.GetDelegateForFunctionPointer(def, typeof(a));
            byteArrayResource = (byte[])All.val.locals[1];
            var array = new OpCode[256];
            var array2 = new OpCode[256];
            oneByteOpCodes = array;
            twoByteOpCodes = array2;
            Starter();
        }
        public static string Str(string text)
        {
            var result = new StringBuilder();
            for (int c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)"^Meow^/"[c % "^Meow^/".Length]));
            return result.ToString();
        }
        public static void Starter()
        {
            var typeFromHandle = typeof(OpCode);
            var typeFromHandle2 = typeof(OpCodes);
            foreach (var fieldInfo in typeFromHandle2.GetFields())
                if (fieldInfo.FieldType == typeFromHandle)
                {
                    var opCode = (OpCode)fieldInfo.GetValue(null);
                    var num = (ushort)opCode.Value;
                    if (opCode.Size == 1)
                    {
                        var b = (byte)num;
                        oneByteOpCodes[b] = opCode;
                    }
                    else
                    {
                        var b2 = (byte)(num | 65024);
                        twoByteOpCodes[b2] = opCode;
                    }
                }
        }

        private static byte[] extractResource(string resourceName)
        {
            using (Stream stream = callingModule.Assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    return array;

                }
            }
        }

        private static string tempFolder = "";

        /// <summary>
        /// Extract DLLs from resources to temporary folder
        /// </summary>
        /// <param name="dllName">name of DLL file to create (including dll suffix)</param>
        /// <param name="resourceBytes">The resource name (fully qualified)</param>
        public static void ExtractEmbeddedDlls(string dllName, byte[] resourceBytes)
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            string[] names = assem.GetManifestResourceNames();
            AssemblyName an = assem.GetName();

            // The temporary folder holds one or more of the temporary DLLs
            // It is made "unique" to avoid different versions of the DLL or architectures.
            tempFolder = String.Format("{0}.{1}.{2}", an.Name, an.ProcessorArchitecture, an.Version);

            string dirName = Path.Combine(Path.GetTempPath(), tempFolder);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // Add the temporary dirName to the PATH environment variable (at the head!)
            string path = Environment.GetEnvironmentVariable("PATH");
            string[] pathPieces = path.Split(';');
            bool found = false;
            foreach (string pathPiece in pathPieces)
            {
                if (pathPiece == dirName)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Environment.SetEnvironmentVariable("PATH", dirName + ";" + path);
            }

            // See if the file exists, avoid rewriting it if not necessary
            string dllPath = Path.Combine(dirName, dllName);
            bool rewrite = true;
            if (File.Exists(dllPath))
            {
                byte[] existing = File.ReadAllBytes(dllPath);
                if (Equality(resourceBytes, existing))
                {
                    rewrite = false;
                }
            }
            if (rewrite)
            {
                File.WriteAllBytes(dllPath, resourceBytes);
            }
        }
        public static bool Equality(byte[] a1, byte[] b1)
        {
            int i;
            if (a1.Length == b1.Length)
            {
                i = 0;
                while (i < a1.Length && (a1[i] == b1[i])) //Earlier it was a1[i]!=b1[i]
                {
                    i++;
                }
                if (i == a1.Length)
                {
                    return true;
                }
            }

            return false;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string dllToLoad, IntPtr hFile, uint flags);

        /// <summary>
        /// managed wrapper around LoadLibrary
        /// </summary>
        /// <param name="dllName"></param>
        static public IntPtr LoadDll(string dllName)
        {
            if (tempFolder == string.Empty)
            {
                throw new Exception("Please call ExtractEmbeddedDlls before LoadDll");
            }
            IntPtr h = LoadLibraryEx(dllName, IntPtr.Zero, 0);
            if (h == IntPtr.Zero)
            {
                Exception e = new Win32Exception();
                throw new DllNotFoundException("Unable to load library: " + dllName + " from " + tempFolder, e);
            }
            return h;
        }
    }
}
