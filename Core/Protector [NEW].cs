using System;
using System.Linq;
using System.Reflection;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Core.Properties;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using Core.Protection;
using Core.Injection;
using dnlib.PE;
using MeoxDLibHelper;
using MethodAttributes = dnlib.DotNet.MethodAttributes;
using MethodImplAttributes = dnlib.DotNet.MethodImplAttributes;
using System.Text.RegularExpressions;
using dnlib;
using System.Collections.Generic;

namespace Core
{
    public class Protector
    {
        public static string path2;

        public static ModuleDefMD moduleDefMD { get; set; }
        public static string name { get; private set; }

        public static bool StringEncryption = false;

        public static bool MathInt = false;

        public static bool NumObf = false;

        public static bool Mutation = false;

        public static int StringAnalysis = 0;

        public static void XORDataWithKeys(byte[] data, byte[] Keys)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ Keys[i % Keys.Length]);
            }
        }

        public static void Settings(bool strien, bool math, bool numob, bool mutation, int stringasly)
        {
            StringEncryption = strien;
            MathInt = math;
            NumObf = numob;
            Mutation = mutation;
            StringAnalysis = stringasly;
        }
        public static List<string> usedMethodsFullNames = new List<string>();
        public static byte[] Protect(byte[] assemblyData, List<string> LstMethod)//public static byte[] Protect(byte[] assemblyData, bool strien , bool math,bool numob,bool mutation, int stringasly)
        {
            
            name = "MeoxD^!"; //Key


            MethodProccesor.AllMethods.Clear();
            moduleDefMD = ModuleDefMD.Load(assemblyData); //load the unprotected binary in dnlib
            
            //InjectInitialise.Watermark();
            asmRefAdder(); //this will resolve references (dlls) such as mscorlib and any dlls the unprotected binary may use. this will be to make sure resolving methods/types/fields in another assembly can be correctly identified
            /*foreach (MethodDef methodDef in moduleDefMD.GetTypes())
            {
                Protector.usedMethodsFullNames.Add(methodDef.FullName);
            }*/
            usedMethodsFullNames.Clear();
            foreach (var typeDef in moduleDefMD.Types)
            {
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    if (LstMethod.Contains(methodDef.FullName))
                    {
                        Protector.usedMethodsFullNames.Add(methodDef.FullName);
                    }
                }
            }
            foreach (var moduleDef in moduleDefMD.GetTypes())
            {
                foreach (var method in moduleDef.Methods)
                {
                    method.ImplAttributes |= MethodImplAttributes.NoInlining;
                   // method.ImplAttributes |= MethodImplAttributes.NoOptimization;
                }
            }
            Injection.InjectInitialise.initaliseMethod();
            Protection.MethodProccesor.ModuleProcessor(); //this will process the module


           EmbeddedResource emv = new EmbeddedResource("MeoxD=-", (Properties.Resources.NativeEncoderx86), ManifestResourceAttributes.Public);
            moduleDefMD.Resources.Add(emv);

            EmbeddedResource emv64 = new EmbeddedResource("MeoxD=+", (Resources.NativeEncoderx64), ManifestResourceAttributes.Public);
            moduleDefMD.Resources.Add(emv64);

            byte[] cleanConversion = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Runtime.dll"));

            XORDataWithKeys(cleanConversion, Encoding.UTF8.GetBytes(@"wMeowKey"));

            EmbeddedResource embc = new EmbeddedResource("+!$#%", cleanConversion, ManifestResourceAttributes.Public); //Full
            moduleDefMD.Resources.Add(embc);

            byte[] cleanXor = Resources.XorMethod;
            //XORDataWithKeys(cleanXor, Encoding.UTF8.GetBytes(@"wMeowKey"));

            EmbeddedResource emb = new EmbeddedResource(Protector.name, cleanXor, ManifestResourceAttributes.Public);
            //EmbeddedResource emb = new EmbeddedResource(Protector.name, Resources.XorMethod, ManifestResourceAttributes.Public); //XorMethod
            moduleDefMD.Resources.Add(emb);

            /* Writing */
            ModuleWriterOptions modOpts = new ModuleWriterOptions(moduleDefMD);
            modOpts.MetaDataOptions.Flags =
             MetaDataFlags
              .PreserveAll; //we need to preserve all metadata tokens, otherwise resolving tokens to methods will not match the originals
            modOpts.MetaDataLogger =
             DummyLogger
              .NoThrowInstance; //since we make an unverifiable module dnlib will throw an exception. the reason we do this is because when using publically available tools this may crash them when trying to save the module.
            MemoryStream mem = new MemoryStream();
            moduleDefMD.Write(mem, modOpts); //save the module.
            return mem.ToArray();

        }
        private static void asmRefAdder()
        {
            var asmResolver = new AssemblyResolver { EnableTypeDefCache = true };
            var modCtx = new ModuleContext(asmResolver);
            asmResolver.DefaultModuleContext = modCtx;
            var asmRefs = moduleDefMD.GetAssemblyRefs().ToList();
            moduleDefMD.Context = modCtx;
            foreach (var asmRef in asmRefs)
            {
                try
                {
                    if (asmRef == null)
                        continue;
                    var asm = asmResolver.Resolve(asmRef.FullName, moduleDefMD);
                    if (asm == null)
                        continue;
                    moduleDefMD.Context.AssemblyResolver.AddToCache(asm);

                }
                catch { }
            }
        }
    }
}
