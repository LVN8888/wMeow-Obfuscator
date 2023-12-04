using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helper;

namespace Protections
{
    public static class AntiDebug
	{
        public static void Inject(ModuleDef moduleDef)
		{
			ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(Runtime.AntiDebugRT).Module);
			TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(Runtime.AntiDebugRT).MetadataToken));
			IEnumerable<IDnlibDef> source = InjectHelper.Inject(typeDef, moduleDef.EntryPoint.DeclaringType, moduleDef);
			MethodDef method2 = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Detected");
            moduleDef.EntryPoint.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(method2));
		}
	}
}
