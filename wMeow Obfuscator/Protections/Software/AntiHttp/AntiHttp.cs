using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace wMeow_Obfuscator.Protections.Software
{
    public class AntiHttp
    {
        public static void Inject(ModuleDef module)
        {
            MethodDef methodDef = (MethodDef)InjectHelper.Inject(ModuleDefMD.Load(typeof(AntiHttpRuntime).Module).ResolveTypeDef(MDToken.ToRID(typeof(AntiHttpRuntime).MetadataToken)),
                module.EntryPoint.DeclaringType,
                module
            ).Single((IDnlibDef method) => method.Name == "Initialize");

            module.EntryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(methodDef));
        }

    }
}
