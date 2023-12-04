using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeoxDLibHelper
{
    public class StringEncryption
    {
        private static Random _random = new Random();
        public static void Execute(ModuleDef module)
        {
            foreach (TypeDef type in module.GetTypes())
            {
                if (type != module.GlobalType)
                {
                    foreach (MethodDef m in type.Methods.Where(x => x.HasBody))
                    {
                        m.Body.MaxStack = 65535;
                        m.Body.SimplifyMacros(m.Parameters);
                        m.Body.SimplifyBranches();
                        List<Instruction> list = new List<Instruction>();
                        Dictionary<Instruction, Instruction> dictionary = new Dictionary<Instruction, Instruction>();
                        foreach (Instruction instruction in m.Body.Instructions)
                        {
                            List<Instruction> list2 = CreateInstruction(instruction, m);
                            dictionary.Add(instruction, list2.First<Instruction>());
                            list.AddRange(list2);
                        }
                        m.Body.SetNewInstructions(list, dictionary);
                    }
                }

            }
        }
        public static void ExecuteMethod(MethodDef m)
        {

            m.Body.MaxStack = 65535;
            m.Body.SimplifyMacros(m.Parameters);
            m.Body.SimplifyBranches();
            List<Instruction> list = new List<Instruction>();
            Dictionary<Instruction, Instruction> dictionary = new Dictionary<Instruction, Instruction>();
            foreach (Instruction instruction in m.Body.Instructions)
            {
                List<Instruction> list2 = CreateInstruction(instruction, m);
                dictionary.Add(instruction, list2.First<Instruction>());
                list.AddRange(list2);
            }
            m.Body.SetNewInstructions(list, dictionary);
        }
    
        private static List<Instruction> CreateInstruction(Instruction instruction, MethodDef m)
        {
            if (instruction.OpCode == OpCodes.Ldstr)
            {
                var loc = new Local(m.Module.CorLibTypes.Object);
                m.Body.Variables.Add(loc);
                string text = (string)instruction.Operand;
                List<Instruction> list = new List<Instruction>();
                list.Add(Instruction.Create(OpCodes.Ldc_I4, text.Length));
                list.Add(Instruction.Create(OpCodes.Newarr, m.Module.CorLibTypes.Char.ToTypeDefOrRef()));
                list.Add(Instruction.Create(OpCodes.Dup));
                list.Add(Instruction.Create(OpCodes.Stloc, loc));
                for (int i = 0; i < text.Length; i++)
                {
                    list.Add(Instruction.Create(OpCodes.Ldloc, loc));
                    list.Add(Instruction.Create(OpCodes.Ldc_I4, i));
                    uint num = (uint)_random.Next(int.MaxValue);
                    list.Add(Instruction.Create(OpCodes.Ldc_I4, (int)num));
                    list.Add(Instruction.Create(OpCodes.Ldc_I4, (int)((uint)text[i] ^ num)));
                    list.Add(Instruction.Create(OpCodes.Xor));
                    list.Add(Instruction.Create(OpCodes.Stelem_I2));
                }
                list.Add(Instruction.Create(OpCodes.Newobj, m.Module.Import(typeof(string).GetConstructor(new Type[]
                {
                    typeof(char[])
                }))));
                return list;
            }
            return new List<Instruction>
            {
                instruction
            };
        }
    }
}
