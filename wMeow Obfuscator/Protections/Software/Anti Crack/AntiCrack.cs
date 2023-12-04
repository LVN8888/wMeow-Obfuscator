using dnlib.DotNet;
using dnlib.DotNet.Emit;
using MeoxDLibHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wMeow_Obfuscator.Protections.Software
{
    public class AntiCrack
    {
        public static void Inject(ModuleDef module)
        {
            MethodDef methodDef = (MethodDef)InjectHelper.Inject(ModuleDefMD.Load(typeof(AntiCrackRuntime).Module).ResolveTypeDef(MDToken.ToRID(typeof(AntiCrackRuntime).MetadataToken)),
                module.EntryPoint.DeclaringType,
                module
            ).Single((IDnlibDef method) => method.Name == "InitCrack");

            module.EntryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(methodDef));
            RemoveObfuscator.ExecuteToken(module, methodDef.MDToken.ToString());
        }
    }
}
