using dnlib.DotNet.Writer;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoxDLibHelper
{
    public class MLib
    {
        public static string name;

        public enum saveMode
        {
            Normal,
            x86
        }
        public class MeoLibrary
        {
            public MeoLibrary(string file)
            {
                MLib.MeoLibrary.filePath = file;
                MLib.MeoLibrary.assembly = AssemblyDef.Load(file, MLib.MeoLibrary.mcontext);
                MLib.MeoLibrary.moduleDef = MLib.MeoLibrary.assembly.ManifestModule;
                MLib.MeoLibrary.globalType = MLib.MeoLibrary.assembly.ManifestModule.GlobalType;
                MLib.MeoLibrary.ctor = MLib.MeoLibrary.assembly.ManifestModule.GlobalType.FindOrCreateStaticConstructor();
                MLib.MeoLibrary.noThrowInstance = false;
                MLib.MeoLibrary.nativeModuleWriterOptions = new NativeModuleWriterOptions(MLib.MeoLibrary.moduleDef as ModuleDefMD)
                {
                    MetaDataLogger = DummyLogger.NoThrowInstance
                };
                MLib.MeoLibrary.moduleWriterOptions = new ModuleWriterOptions(MLib.MeoLibrary.moduleDef)
                {
                    MetaDataLogger = DummyLogger.NoThrowInstance
                };
            }

            static string NewName()
            {
                return string.Concat(new string[]
                {
                    Path.GetDirectoryName(MLib.MeoLibrary.filePath),
                    "//",
                    Path.GetFileNameWithoutExtension(MLib.MeoLibrary.filePath),
                    "_Protected",
                    Path.GetExtension(MLib.MeoLibrary.filePath)
                });
            }

            public static void buildASM(MLib.saveMode mode)
            {
                if (mode == MLib.saveMode.Normal)
                {
                    MLib.MeoLibrary.moduleWriterOptions.MetaDataOptions.Flags = (MetaDataFlags.AlwaysCreateGuidHeap | MetaDataFlags.AlwaysCreateStringsHeap | MetaDataFlags.AlwaysCreateUSHeap | MetaDataFlags.AlwaysCreateBlobHeap);
                    MLib.MeoLibrary.moduleDef.Write(MLib.MeoLibrary.NewName(), MLib.MeoLibrary.moduleWriterOptions);
                    return;
                }
                if (mode == MLib.saveMode.x86)
                {
                    MLib.MeoLibrary.nativeModuleWriterOptions.MetaDataOptions.Flags = (MetaDataFlags.AlwaysCreateGuidHeap | MetaDataFlags.AlwaysCreateStringsHeap | MetaDataFlags.AlwaysCreateUSHeap | MetaDataFlags.AlwaysCreateBlobHeap);
                    MLib.MeoLibrary.nativeModuleWriterOptions.MetaDataLogger = DummyLogger.NoThrowInstance;
                    (MLib.MeoLibrary.moduleDef as ModuleDefMD).NativeWrite(MLib.MeoLibrary.NewName(), MLib.MeoLibrary.nativeModuleWriterOptions);
                    MLib.name = MLib.MeoLibrary.NewName();
                }
            }

            public static string filePath;

            public static AssemblyDef assembly;

            public static ModuleDef moduleDef;

            public static ModuleContext mcontext;

            public static TypeDef globalType;

            public static MethodDef ctor;

            public static NativeModuleWriterOptions nativeModuleWriterOptions;

            public static ModuleWriterOptions moduleWriterOptions;

            public static bool noThrowInstance;
        }

        public class Meo
        {
            public Meo(string filePath)
            {
                new MLib.MeoLibrary(filePath);
            }

            public static void Save()
            {
                MLib.MeoLibrary.buildASM(MLib.saveMode.x86);
            }
        }

        public class MeoClean
        {
            public static void Clean()
            {
                MLib.MeoLibrary.assembly = null;
                MLib.MeoLibrary.filePath = null;
                MLib.MeoLibrary.ctor = null;
                MLib.MeoLibrary.globalType = null;
                MLib.MeoLibrary.mcontext = null;
                MLib.MeoLibrary.moduleDef = null;
                MLib.MeoLibrary.moduleWriterOptions = null;
                MLib.MeoLibrary.nativeModuleWriterOptions = null;
                MLib.MeoLibrary.noThrowInstance = false;
            }
        }
    }
}
