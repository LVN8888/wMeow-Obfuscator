using dnlib.DotNet.Emit;
using dnlib.DotNet;
using Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MeoxDLibHelper.Resource
{
    public class ProxyResource : Randomizer
    {
        public static void Execute(ModuleDef moduleDefMd)
        {
            var typeModule = ModuleDefMD.Load(typeof(RwMeowS).Module);
            var typeDef2 = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(RwMeowS).MetadataToken));

            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef2, moduleDefMd.GlobalType, moduleDefMd);
            var decoderMethod = (MethodDef)members.Single(method => method.Name == "ByteResource");


            
            foreach (var md in moduleDefMd.GlobalType.Methods)
            {
                if (md.Name != ".ctor") continue;
                moduleDefMd.GlobalType.Remove(md);
                break;
            }
            foreach (TypeDef type in moduleDefMd.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody || method.Body == null) continue;
                    IList<Instruction> instr = method.Body.Instructions;
                    for (int i = 0; i < instr.Count; i++)
                    {
                        try
                        {
                            if (instr[i].OpCode != OpCodes.Ldstr) continue;
                            var resourceName = GenerateRandomString3(Randomizer.Next(3, 1));

                            moduleDefMd.Resources.Add(new EmbeddedResource(resourceName, Encoding.UTF8.GetBytes(instr[i].Operand.ToString())));
                            instr[i].Operand = resourceName;
                            instr.Insert(i + 1, Instruction.Create(OpCodes.Call, decoderMethod));
                        }
                        catch
                        {
                        }
                    }
                }
            }
            foreach (TypeDef type in moduleDefMd.GetTypes())
            {
                if (type.Name == "<Module>")
                {
                    foreach (MethodDef methodDef in type.Methods)
                    {
                        if (methodDef.Name == "ByteResource")
                        {
                            Instruction operand = methodDef.Body.Instructions[methodDef.Body.Instructions.Count - 1];
                            methodDef.Body.Instructions.Insert(1, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[1]));
                            methodDef.Body.Instructions.Insert(2, new Instruction(OpCodes.Unaligned, 0));
                            methodDef.Body.Instructions.Insert(3, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[1]));
                            methodDef.Body.Instructions.Insert(4, new Instruction(OpCodes.Unaligned, 0));
                            methodDef.Body.Instructions.Insert(5, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[1]));
                            methodDef.Body.Instructions.Insert(6, new Instruction(OpCodes.Unaligned, 0));
                            methodDef.Body.Instructions.Insert(7, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[1]));
                            methodDef.Body.Instructions.Insert(8, new Instruction(OpCodes.Unaligned, operand));
                        }
                    }
                }
            }
        }
    }
}
