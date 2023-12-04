using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Protections
{
	public static class zAttributes
	{
		public static void Add(ModuleDef M)
		{
			ModuleDef moduleDef = M;
			List<string> list = new List<string>
			{
               "wMeow",
               "Protected_By_100009799573857",
               "I_Decline_To_Unpack_Files_That_Are_Protected_By_wMeow"
               //"FB.Com/iuBn.045692"//FB.Com/iuBn.045692
            };
			foreach (string s in list)
			{
				TypeDef typeDef = new TypeDefUser("Credits", s, moduleDef.Import(typeof(Attribute)));
				typeDef.Attributes = TypeAttributes.NotPublic;
				M.Types.Add(typeDef);
			}
            /*foreach (string s in list)
            {
                TypeDef typeDef1 = new TypeDefUser(s, "", moduleDef.Import(typeof(Attribute)));
                typeDef1.Attributes = TypeAttributes.NotPublic;
                M.Types.Add(typeDef1);
            }*/
            
            TypeDef typeDef2 = M.Types[new Random().Next(0, moduleDef.Types.Count)];

		}
        public static void Watermark(ModuleDef Module)
        {
            TypeRef typeRef = Module.CorLibTypes.GetTypeRef("System", "Attribute");
            TypeDefUser typeDefUser = new TypeDefUser("", "wMeowObfuscator", typeRef);
            Module.Types.Add(typeDefUser);
            MethodDefUser methodDefUser = new MethodDefUser(".ctor", MethodSig.CreateInstance(Module.CorLibTypes.Void, Module.CorLibTypes.String), dnlib.DotNet.MethodImplAttributes.IL, dnlib.DotNet.MethodAttributes.FamANDAssem | dnlib.DotNet.MethodAttributes.Family | dnlib.DotNet.MethodAttributes.HideBySig | dnlib.DotNet.MethodAttributes.SpecialName | dnlib.DotNet.MethodAttributes.RTSpecialName);
            methodDefUser.Body = new CilBody();
            methodDefUser.Body.MaxStack = 1;
            methodDefUser.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            methodDefUser.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(Module, ".ctor", MethodSig.CreateInstance(Module.CorLibTypes.Void), typeRef)));
            methodDefUser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
            typeDefUser.Methods.Add(methodDefUser);
            CustomAttribute customAttribute = new CustomAttribute(methodDefUser);
            customAttribute.ConstructorArguments.Add(new CAArgument(Module.CorLibTypes.String, "wMeow")); //www.facebook.com/iuBn.045692
            Module.CustomAttributes.Add(customAttribute);
        }
    }
}
