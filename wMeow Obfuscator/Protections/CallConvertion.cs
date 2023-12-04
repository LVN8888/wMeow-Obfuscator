using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MeoxDLibHelper;

namespace Protections
{
    internal class CallConvertion
    {
        public static void Execute()
        {
            Local local = new Local(MLib.MeoLibrary.ctor.Module.ImportAsTypeSig(typeof(Module)));
            MLib.MeoLibrary.ctor.Body.Variables.Add(local);
            FieldDef fieldDef = new FieldDefUser(RandomString(20), new FieldSig(MLib.MeoLibrary.moduleDef.ImportAsTypeSig(typeof(IntPtr[]))), dnlib.DotNet.FieldAttributes.FamANDAssem | dnlib.DotNet.FieldAttributes.Family | dnlib.DotNet.FieldAttributes.Static);
            MLib.MeoLibrary.moduleDef.GlobalType.Fields.Add(fieldDef);
            List<Instruction> list = new List<Instruction>
            {
                OpCodes.Ldtoken.ToInstruction(MLib.MeoLibrary.moduleDef.GlobalType),
                OpCodes.Call.ToInstruction(MLib.MeoLibrary.moduleDef.Import(typeof(Type).GetMethod("GetTypeFromHandle", new Type[]
                {
                    typeof(RuntimeTypeHandle)
                }))),
                OpCodes.Callvirt.ToInstruction(MLib.MeoLibrary.moduleDef.Import(typeof(Type).GetMethod("get_Module"))),
                OpCodes.Stloc.ToInstruction(local),
                OpCodes.Ldc_I4.ToInstruction(666),
                OpCodes.Newarr.ToInstruction(MLib.MeoLibrary.moduleDef.CorLibTypes.IntPtr),
                OpCodes.Stsfld.ToInstruction(fieldDef)
            };
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            int num = 0;
            foreach (TypeDef typeDef in MLib.MeoLibrary.moduleDef.Types.ToArray<TypeDef>())
            {
                foreach (MethodDef methodDef in typeDef.Methods.ToArray<MethodDef>())
                {
                    if (!methodDef.IsConstructor)
                    {
                        if (methodDef.Body != null)
                        {
                            if (methodDef.HasBody && methodDef.Body.HasInstructions && !methodDef.IsConstructor && !methodDef.DeclaringType.IsGlobalModuleType)
                            {
                                IList<Instruction> instructions = methodDef.Body.Instructions;
                                int k = 0;
                                while (k < instructions.Count<Instruction>())
                                {
                                    MemberRef memberRef = null;
                                    bool flag3;
                                    if (instructions[k].OpCode == OpCodes.Call || instructions[k].OpCode == OpCodes.Callvirt)
                                    {
                                        memberRef = (instructions[k].Operand as MemberRef);
                                        flag3 = (memberRef != null);
                                    }
                                    else
                                    {
                                        flag3 = false;
                                    }
                                    if (flag3)
                                    {
                                        if (!memberRef.HasThis)
                                        {
                                            int key = memberRef.MDToken.ToInt32();
                                            if (!dictionary.ContainsKey(key))
                                            {
                                                list.Add(OpCodes.Ldsfld.ToInstruction(fieldDef));
                                                list.Add(OpCodes.Ldc_I4.ToInstruction(num));
                                                list.Add(OpCodes.Ldftn.ToInstruction(memberRef));
                                                list.Add(OpCodes.Stelem_I.ToInstruction());
                                                list.Add(OpCodes.Nop.ToInstruction());
                                                instructions[k].OpCode = OpCodes.Ldsfld;
                                                instructions[k].Operand = fieldDef;
                                                instructions.Insert(++k, Instruction.Create(OpCodes.Ldc_I4, num));
                                                instructions.Insert(++k, Instruction.Create(OpCodes.Ldelem_I));
                                                instructions.Insert(++k, Instruction.Create(OpCodes.Calli, memberRef.MethodSig));
                                                dictionary.Add(key, num);
                                                num++;
                                            }
                                            else
                                            {
                                                int value;
                                                dictionary.TryGetValue(key, out value);
                                                instructions[k].OpCode = OpCodes.Ldsfld;
                                                instructions[k].Operand = fieldDef;
                                                instructions.Insert(++k, Instruction.Create(OpCodes.Ldc_I4, value));
                                                instructions.Insert(++k, Instruction.Create(OpCodes.Ldelem_I));
                                                instructions.Insert(++k, Instruction.Create(OpCodes.Calli, memberRef.MethodSig));
                                            }
                                        }
                                    }
                                    k++;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            list[4].OpCode = OpCodes.Ldc_I4;
            list[4].Operand = num;
            for (int l = 0; l < list.Count; l++)
            {
                MLib.MeoLibrary.ctor.Body.Instructions.Insert(l, list[l]);
            }
        }
        public static Random Random = new Random();

        private static string RandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()-_+=[{}]|;:',<.>/?"
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
