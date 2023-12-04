using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dnlib.PE;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Core.Protection;
using MeoxDLibHelper;
using System;
using TypeAttributes = dnlib.DotNet.TypeAttributes;
using MethodAttributes = dnlib.DotNet.MethodAttributes;
using MethodImplAttributes = dnlib.DotNet.MethodImplAttributes;

namespace Core.Injection
{
    class InjectInitialise
    {
        public static MemberRef conversionInit;

        public static MemberRef convertBack;

        public static void initaliseMethod()
        {

            byte[] conversionPlain = System.IO.File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Runtime.dll"));
            conversionAssembly = Assembly.Load(conversionPlain).ManifestModule;

            conversionDef = ModuleDefMD.Load(conversionPlain);

        }
        public static void injectIntoCctor()
        {
            foreach (TypeDef t in conversionDef.Types)
            {
                foreach (MethodDef m in t.Methods)
                {
                    if (m.Name == "Initalize")
                    {
                        conversionInit = (MemberRef)Protector.moduleDefMD.Import(m);
                    }
                    if (m.Name == "Meoww")
                    {
                        convertBack = (MemberRef)Protector.moduleDefMD.Import(m);
                    }
                }
            }

            var a = typeof(Resource);
            var asm = ModuleDefMD.Load(typeof(Resource).Assembly.Location);
            var tester2 = asm.GetTypes();
            var abc = InjectHelper.Inject(tester2.ToArray()[13], Protector.moduleDefMD.GlobalType, Protector.moduleDefMD);
            foreach (MethodDef md in Protector.moduleDefMD.GlobalType.Methods)
            {
                if (md.Name == ".ctor")
                {
                    Protector.moduleDefMD.GlobalType.Remove(md);
                    break;
                }
            }
            if (Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor().Body == null)
            {
                var cil = new CilBody();


                cil.Instructions.Add(new Instruction(OpCodes.Call, Protector.moduleDefMD.Types[0].Methods[0]));

                cil.Instructions.Add(new Instruction(OpCodes.Ret));
                Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor().Body = cil;
            }
            else
            {
                var vody = Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor().Body;
                vody.Instructions.Insert(0, new Instruction(OpCodes.Call, Protector.moduleDefMD.Types[0].Methods.Where(i => i.Name == "setup").First()));
                foreach (TypeDef type in Protector.moduleDefMD.GetTypes())
                {
                    if (type.Name == "<Module>")
                    {
                        foreach (MethodDef methodDef in type.Methods)
                        {
                            if (methodDef.Name == "setup" || methodDef.Name == "ADum" || methodDef.Name == "SERT" || methodDef.Name == ".cctor" ||methodDef.Name == "ResolveAssembly"||methodDef.Name == "decrypt"|| methodDef.Name== "Base64" || methodDef.Name == "ByteResource")
                            {
                                Instruction operand = methodDef.Body.Instructions[methodDef.Body.Instructions.Count - 1];
                                methodDef.Body.Instructions.Insert(1, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[1]));
                                methodDef.Body.Instructions.Insert(2, new Instruction(OpCodes.Unaligned, 0));
                                methodDef.Body.Instructions.Insert(3, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[1]));
                                methodDef.Body.Instructions.Insert(4, new Instruction(OpCodes.Unaligned, operand));
                            }
                        }
                    }
                }
                if ((Protector.moduleDefMD.Characteristics & Characteristics.Dll) != 0)
                {

                    vody.Instructions.Insert(0, new Instruction(OpCodes.Ldnull));
                    vody.Instructions.Insert(1, new Instruction(OpCodes.Ldstr, MethodProccesor.Str("+!$"))); //+!$%#
                    vody.Instructions.Insert(2, new Instruction(OpCodes.Ldstr, "d01lb3c="));
                    
                    vody.Instructions.Insert(3, new Instruction(OpCodes.Ldstr, MethodProccesor.Str("MeoxD^!"))); //MeoxD^!
                    vody.Instructions.Insert(4, new Instruction(OpCodes.Call, InjectInitialise.conversionInit));
                }
            }
        }
        public static void InjectMethod(int pos, int id, int size, MethodDef meth)
        {
            var containsOut = false;
            meth.Body.Instructions.Clear();
            var rrr = meth.Parameters.Where(i => i.Type.FullName.EndsWith("&"));
            if (rrr.Count() != 0)
                containsOut = true;

            var rrg = new Local(Protector.moduleDefMD.CorLibTypes.Object.ToSZArraySig());
            var loc = new Local(Protector.moduleDefMD.CorLibTypes.Object);
            var loc2 = new Local(new SZArraySig(Protector.moduleDefMD.CorLibTypes.Object));
            var cli = new CilBody();
            foreach (var bodyVariable in meth.Body.Variables)
                cli.Variables.Add(bodyVariable);
            cli.Variables.Add(rrg);
            cli.Variables.Add(loc);
            cli.Variables.Add(loc2);
            var outParams = new List<Local>();
            var testerDictionary = new Dictionary<Parameter, Local>();
            if (containsOut)
                foreach (var parameter in rrr)
                {
                    var locf = new Local(parameter.Type.Next);
                    testerDictionary.Add(parameter, locf);
                    cli.Variables.Add(locf);
                }

            cli.Instructions.Add(new Instruction(OpCodes.Ldstr,  Xoring2(Encode(size.ToString() + "-" + pos.ToString()))));

            cli.Instructions.Add(new Instruction(OpCodes.Ldstr, Xoring(Encode(id.ToString()))));
            cli.Instructions.Add(new Instruction(OpCodes.Ldtoken, meth.DeclaringType));

            cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, meth.Parameters.Count));
            cli.Instructions.Add(new Instruction(OpCodes.Newarr, Protector.moduleDefMD.CorLibTypes.Object.ToTypeDefOrRef()));
            for (var i = 0; i < meth.Parameters.Count; i++)
            {
                var par = meth.Parameters[i];
                cli.Instructions.Add(new Instruction(OpCodes.Dup));
                cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, i));
                if (containsOut)
                {
                    if (rrr.Contains(meth.Parameters[i]))
                    {
                        cli.Instructions.Add(new Instruction(OpCodes.Ldloc, testerDictionary[meth.Parameters[i]]));
                    }
                    else
                    {
                        cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[i]));
                    }
                }
                else
                    cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[i]));

                cli.Instructions.Add(par.Type.FullName.EndsWith("&") ? new Instruction(OpCodes.Box, par.Type.Next.ToTypeDefOrRef()) : new Instruction(OpCodes.Box, par.Type.ToTypeDefOrRef()));
                cli.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
            }
            /*cli.Instructions.Add(new Instruction(OpCodes.Ldloc_0));*/
            //cli.Instructions.Add(new Instruction(OpCodes.Ldnull));

            //cli.Instructions.Add(new Instruction(OpCodes.Ceq));


            cli.Instructions.Add(new Instruction(OpCodes.Call, convertBack));



          

            if (meth.ReturnType.ElementType == ElementType.Void)
                cli.Instructions.Add(Instruction.Create(OpCodes.Pop));
            else if (meth.ReturnType.IsValueType)
                cli.Instructions.Add(Instruction.Create(OpCodes.Unbox_Any, meth.ReturnType.ToTypeDefOrRef()));
            else
                cli.Instructions.Add(Instruction.Create(OpCodes.Castclass, meth.ReturnType.ToTypeDefOrRef()));

            if (containsOut)
            {
                foreach (var parameter in rrr)
                {

                    cli.Instructions.Add(new Instruction(OpCodes.Ldarg, parameter));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldloc, loc));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldloc, loc2));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, meth.Parameters.IndexOf(parameter)));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldelem_Ref));
                    cli.Instructions.Add(new Instruction(OpCodes.Unbox_Any, parameter.Type.Next.ToTypeDefOrRef()));
                    cli.Instructions.Add(new Instruction(OpCodes.Stind_Ref));


                }
                cli.Instructions.Add(new Instruction(OpCodes.Ret));
            }
            else
                cli.Instructions.Add(new Instruction(OpCodes.Ret));

            meth.Body = cli;
            meth.Body.UpdateInstructionOffsets();
            meth.Body.MaxStack += 10;
            
            
            


            /*if (meth.Name != "<Module>")
            {
                ///meth.Name = GenerateRandomString(100);
                cli.Instructions.Insert(1, new Instruction(OpCodes.Br_S, cli.Instructions[1]));
                cli.Instructions.Insert(2, new Instruction(OpCodes.Unaligned, 0));

            }*/




        }
        public static void Watermark()
        {
            TypeRef attr = Protector.moduleDefMD.CorLibTypes.GetTypeRef("System", "Attribute");
            var attr2 = new TypeDefUser("", "wMeow", attr);
            Protector.moduleDefMD.Types.Add(attr2);
            //MemberRefUser ctor2 = new MemberRefUser(Protector.moduleDefMD, ".ctor", MethodSig.CreateInstance(Protector.moduleDefMD.CorLibTypes.Void), attr2);


            var ctor = new MethodDefUser( ".ctor",
            MethodSig.CreateInstance(Protector.moduleDefMD.CorLibTypes.Void, Protector.moduleDefMD.CorLibTypes.String),
            dnlib.DotNet.MethodImplAttributes.Managed,
            dnlib.DotNet.MethodAttributes.HideBySig | dnlib.DotNet.MethodAttributes.Public | dnlib.DotNet.MethodAttributes.SpecialName | dnlib.DotNet.MethodAttributes.RTSpecialName);

            ctor.Body = new CilBody();
            ctor.Body.MaxStack = 1;
            ctor.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            ctor.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(Protector.moduleDefMD, ".ctor", MethodSig.CreateInstance(Protector.moduleDefMD.CorLibTypes.Void), attr)));
            ctor.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
            attr2.Methods.Add(ctor);

            foreach (var type in Protector.moduleDefMD.GetTypes())
            {
                if (type == Protector.moduleDefMD.GlobalType) continue;
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    if (method.IsConstructor) continue;
                    var attr3 = new CustomAttribute(ctor);
                    attr3.ConstructorArguments.Add(new CAArgument(Protector.moduleDefMD.CorLibTypes.String, "Error, wMeow Runtime library not loadded"));
                    method.CustomAttributes.Add(attr3);
                }
            }

            var attr_3 = new CustomAttribute(ctor);
            attr_3.ConstructorArguments.Add(new CAArgument(Protector.moduleDefMD.CorLibTypes.String, "Error, wMeow Runtime library not loadded"));
            Protector.moduleDefMD.CustomAttributes.Add(attr_3);


            TypeRef attrRef = Protector.moduleDefMD.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute");
            var ctorRef = new MemberRefUser(Protector.moduleDefMD, ".ctor", MethodSig.CreateInstance(Protector.moduleDefMD.CorLibTypes.Void), attrRef);

            var attr5 = new CustomAttribute(ctorRef);
            Protector.moduleDefMD.CustomAttributes.Add(attr5);
        }
        
        public static string Xoring(string inputString)
        {
            char c = 'ع';
            string text = "";
            int length = inputString.Length;
            for (int i = 0; i < length; i++)
            {
                text += char.ToString((char)(inputString[i] ^ c));
            }
            return text;
        }
        public static string Xoring2(string inputString)
        {
            char c = 'ح';
            string text = "";
            int length = inputString.Length;
            for (int i = 0; i < length; i++)
            {
                text += char.ToString((char)(inputString[i] ^ c));
            }
            return text;
        }

        public static string Encode(string str)
        {
            string encode = ConvertStringToHex(str);
            encode = encode.Replace("6", "/");
            encode = encode.Replace("3", "-");

            encode = ConvertStringToHex(encode);
            encode = encode.Replace("6", "/");
            encode = encode.Replace("3", "-");

            encode = ConvertStringToHex(encode);
            encode = encode.Replace("2", "/");
            encode = encode.Replace("d", "-");
            encode = encode.Replace("3", ":");
            encode = encode.Replace("f", "%");
            encode = encode.Replace("4", "?");
            return encode;
        }

        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += string.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
        public static System.Reflection.Module conversionAssembly { get; set; }

        public static ModuleDefMD conversionDef { get; set; }
    }
}
