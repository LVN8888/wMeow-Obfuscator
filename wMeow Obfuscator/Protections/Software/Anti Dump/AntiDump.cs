using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helper;
using Runtime;

namespace Protections
{
	public static class AntiDump
	{
        public static void Inject(ModuleDef M)
		{
			ModuleDef moduleDef = M;
			ModuleDefMD moduleDefMD = ModuleDefMD.Load(typeof(AntiDumpRT).Module);
			TypeDef typeDef = moduleDefMD.ResolveTypeDef(MDToken.ToRID(typeof(AntiDumpRT).MetadataToken));
			IEnumerable<IDnlibDef> source = InjectHelper.Inject(typeDef, moduleDef.GlobalType, moduleDef);
			MethodDef method2 = (MethodDef)source.Single((IDnlibDef method) => method.Name == "ADum");
			MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
			methodDef.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(method2));
		}
	}
}
