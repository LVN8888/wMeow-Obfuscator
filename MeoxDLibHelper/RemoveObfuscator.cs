using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoxDLibHelper
{
    public class RemoveObfuscator
    {

        private static Random _random = new Random(Guid.NewGuid().GetHashCode());

        private static List<string> _names = new List<string>();

        public static void Execute(ModuleDef module)
        {
            var importer = new Importer(module);
            foreach (var type in module.GetTypes().Where(t => t.Methods.Count != 0))
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;


                    var instructions = method.Body.Instructions;

                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode != OpCodes.Ldstr)
                            continue;

                        var replaceMethod =
                            importer.Import(typeof(string).GetMethod("Remove", new[] { typeof(int), typeof(int) }) ?? throw new InvalidDataException());

                        string operand = (string)instructions[i].Operand;

                        var result = ObfuscateString(ref operand);

                        instructions[i].Operand = operand;

                        var implant = new List<Instruction>();
                        int count = 0;
                        foreach (var pair in result)
                        {
                            implant.Add(OpCodes.Ldc_I4.ToInstruction(pair.Item1));
                            implant.Add(OpCodes.Ldc_I4.ToInstruction(pair.Item2));
                            implant.Add(count == 0 ? new Instruction(OpCodes.Call, replaceMethod) : new Instruction(OpCodes.Callvirt, replaceMethod));

                            count++;
                        }
                        ((List<Instruction>)instructions).InsertRange(i + 1, implant);


                        i += implant.Count;
                    }

                    instructions.OptimizeMacros();
                }
            }
        }
        public static void ExecuteToken(ModuleDef module, string token)
        {
            var importer = new Importer(module);
            foreach (var type in module.GetTypes().Where(t => t.Methods.Count != 0))
            {
                foreach (var method in type.Methods)
                {
                    if (method.MDToken.ToString() != token) continue;
                    if (!method.HasBody)
                        continue;


                    var instructions = method.Body.Instructions;

                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode != OpCodes.Ldstr)
                            continue;

                        var replaceMethod =
                            importer.Import(typeof(string).GetMethod("Remove", new[] { typeof(int), typeof(int) }) ?? throw new InvalidDataException());

                        string operand = (string)instructions[i].Operand;

                        var result = ObfuscateString(ref operand);

                        instructions[i].Operand = operand;

                        var implant = new List<Instruction>();
                        int count = 0;
                        foreach (var pair in result)
                        {
                            implant.Add(OpCodes.Ldc_I4.ToInstruction(pair.Item1));
                            implant.Add(OpCodes.Ldc_I4.ToInstruction(pair.Item2));
                            implant.Add(count == 0 ? new Instruction(OpCodes.Call, replaceMethod) : new Instruction(OpCodes.Callvirt, replaceMethod));

                            count++;
                        }
                        ((List<Instruction>)instructions).InsertRange(i + 1, implant);


                        i += implant.Count;
                    }

                    instructions.OptimizeMacros();
                }
            }
        }
        public static void ExecuteTwo(MethodDef method)
        {
            var importer = new Importer(method.Module);

            var instructions = method.Body.Instructions;

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].OpCode != OpCodes.Ldstr)
                    continue;

                var replaceMethod =
                    importer.Import(typeof(string).GetMethod("Remove", new[] { typeof(int), typeof(int) }) ?? throw new InvalidDataException());

                string operand = (string)instructions[i].Operand;

                var result = ObfuscateString(ref operand);

                instructions[i].Operand = operand;

                var implant = new List<Instruction>();
                int count = 0;
                foreach (var pair in result)
                {
                    implant.Add(OpCodes.Ldc_I4.ToInstruction(pair.Item1));
                    implant.Add(OpCodes.Ldc_I4.ToInstruction(pair.Item2));
                    implant.Add(count == 0 ? new Instruction(OpCodes.Call, replaceMethod) : new Instruction(OpCodes.Callvirt, replaceMethod));

                    count++;
                }
                ((List<Instruction>)instructions).InsertRange(i + 1, implant);


                i += implant.Count;
            }

            instructions.OptimizeMacros();


        }
        private static List<Tuple<int, int>> ObfuscateString(ref string input)
        {
            int insertsCount = _random.Next(5);
            var result = new List<Tuple<int, int>>();

            for (int i = 0; i < insertsCount; i++)
            {
                int index = _random.Next(0, input.Length);
                string insert = GetRandomName();
                int insertLength = insert.Length;

                input = input.Insert(index, insert);

                result.Add(new Tuple<int, int>(index, insertLength));
            }

            result.Reverse();
            return result;
        }

        private static string GetRandomName()
        {
            if (_names.Count != 0)
            {
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < _random.Next(0, 5); i++)
                {
                    builder.Append(_names[_random.Next(_names.Count)]);
                    if (_random.Next(0, 2) == 0)
                        builder.Append((char)0x20, _random.Next(1, 9)); // Append random spaces
                }

                return builder.ToString();
            }

            var types = typeof(string).Module.GetTypes();

            foreach (var type in types)
            {
                foreach (var method in type.GetMethods())
                {
                    if (!_names.Contains(method.Name))
                        _names.Add(method.Name);
                }
            }

            return GetRandomName();
        }
    }
}
