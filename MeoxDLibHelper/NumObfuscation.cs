using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoxDLibHelper
{
    public class NumObfuscation
    {
        private static Random _random = new Random();

        public static void Execute(ModuleDef m)
        {
            foreach (TypeDef type in m.GetTypes())
            {
                foreach (MethodDef method in type.Methods)
                {
                    ExecuteMethod(method); 
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
                List<Instruction> list2 = CreateInstruction(instruction);
                dictionary.Add(instruction, list2.First<Instruction>());
                list.AddRange(list2);
            }

            m.Body.SetNewInstructions(list, dictionary);
        }
        private static List<Instruction> CreateInstruction(Instruction instruction)
        {
            Code code = instruction.OpCode.Code;
            if (code == Code.Ldc_I4)
            {
                List<Instruction> list = null;
                uint num = (uint)((int)instruction.Operand);

                uint num2 = (uint)(1 + _random.Next(1073741823));
                uint value =  num2 ^ num;
                list = new List<Instruction>
                    {
                         Instruction.Create(OpCodes.Ldc_I4, (int)value),
                         Instruction.Create(OpCodes.Ldc_I4, (int)num2),
                         Instruction.Create(OpCodes.Xor)
                    };
                if (list != null)
                {
                    return list;
                }
            }
            return new List<Instruction>
            {
                instruction
            };
        }
    }
}
