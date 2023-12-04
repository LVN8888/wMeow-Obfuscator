using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace MeoxDLibHelper
{
    public class MutationProtection
    {
        public static void Execute2(ModuleDef module)
        {
            foreach (TypeDef type in (from types in module.Types where types.HasMethods && !types.IsGlobalModuleType select types).ToArray<TypeDef>())
            {
                if (type.Name == "<Module>") continue;
                foreach (MethodDef method in (from methods in type.Methods where methods.HasBody && methods.Body.HasInstructions && methods.Body.Instructions.Count > 1 && !methods.DeclaringType.IsGlobalModuleType select methods).ToArray<MethodDef>())
                {
                    //Mutate7(method);
                    method.Body.SimplifyMacros(method.Parameters);
                    method.Body.SimplifyBranches();


                    Mutate1(method); //DisIntegration
                    /*Mutate5(method); //IntMath
                    Mutate6(method); //ZeroReplacer
                    Mutate7(method); //Number to string*/
                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                    method.Body.SimplifyMacros(method.Parameters);
                    method.Body.OptimizeMacros();
                }
            }
        }

        public static void Mutate1(MethodDef method)
        {
            CilBody body = method.Body;
            body.SimplifyBranches();
            Random rnd = new Random();
            int x = 0;
            while (x < body.Instructions.Count)
            {
                if (body.Instructions[x].IsLdcI4())
                {
                    int original = body.Instructions[x].GetLdcI4Value();
                    int multiplier = rnd.Next(5, 40);
                    body.Instructions[x].OpCode = OpCodes.Ldc_I4;
                    body.Instructions[x].Operand = multiplier * original;
                    body.Instructions.Insert(x + 1, Instruction.Create(OpCodes.Ldc_I4, multiplier));
                    body.Instructions.Insert(x + 2, Instruction.Create(OpCodes.Div));
                    x += 3;
                }
                else
                    x++;
            }
            int num = 0;
            ITypeDefOrRef type = null;
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                Instruction instruction = method.Body.Instructions[i];
                if (instruction.IsLdcI4())
                {
                    switch (rnd.Next(1, 16))
                    {
                        case 1:
                            type = method.Module.Import(typeof(int));
                            num = 4;
                            break;
                        case 2:
                            type = method.Module.Import(typeof(sbyte));
                            num = 1;
                            break;
                        case 3:
                            type = method.Module.Import(typeof(byte));
                            num = 1;
                            break;
                        case 4:
                            type = method.Module.Import(typeof(bool));
                            num = 1;
                            break;
                        case 5:
                            type = method.Module.Import(typeof(decimal));
                            num = 16;
                            break;
                        case 6:
                            type = method.Module.Import(typeof(short));
                            num = 2;
                            break;
                        case 7:
                            type = method.Module.Import(typeof(long));
                            num = 8;
                            break;
                        case 8:
                            type = method.Module.Import(typeof(uint));
                            num = 4;
                            break;
                        case 9:
                            type = method.Module.Import(typeof(float));
                            num = 4;
                            break;
                        case 10:
                            type = method.Module.Import(typeof(char));
                            num = 2;
                            break;
                        case 11:
                            type = method.Module.Import(typeof(ushort));
                            num = 2;
                            break;
                        case 12:
                            type = method.Module.Import(typeof(double));
                            num = 8;
                            break;
                        case 13:
                            type = method.Module.Import(typeof(DateTime));
                            num = 8;
                            break;
                        case 14:
                            type = method.Module.Import(typeof(ConsoleKeyInfo));
                            num = 12;
                            break;
                        case 15:
                            type = method.Module.Import(typeof(Guid));
                            num = 16;
                            break;
                    }
                    int num2 = rnd.Next(1, 1000);
                    bool flag = Convert.ToBoolean(rnd.Next(0, 2));
                    switch ((num != 0) ? ((Convert.ToInt32(instruction.Operand) % num == 0) ? rnd.Next(1, 5) : rnd.Next(1, 4)) : rnd.Next(1, 4))
                    {
                        case 1:
                            method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
                            method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Add));
                            instruction.Operand = Convert.ToInt32(instruction.Operand) - num + (flag ? (-num2) : num2);
                            method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
                            method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
                            i += 4;
                            break;
                        case 2:
                            method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
                            method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Sub));
                            instruction.Operand = Convert.ToInt32(instruction.Operand) + num + (flag ? (-num2) : num2);
                            method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
                            method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
                            i += 4;
                            break;
                        case 3:
                            method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
                            method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Add));
                            instruction.Operand = Convert.ToInt32(instruction.Operand) - num + (flag ? (-num2) : num2);
                            method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
                            method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
                            i += 4;
                            break;
                        case 4:
                            method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
                            method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Mul));
                            instruction.Operand = Convert.ToInt32(instruction.Operand) / num;
                            i += 2;
                            break;
                        default:
                            method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
                            method.Body.Instructions.Insert(i + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
                            i += 4;
                            break;
                    }
                }
            }
        }




        public static List<Instruction> instr = new List<Instruction>();
        static List<int> Integers = new List<int>();
        static List<int> Integers_Position = new List<int>();
        static int Index = 0;
        static Random rnd = new Random();

        public static void Mutate5(MethodDef method)
        {
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                {
                    List<Instruction> instructions = Calc(Convert.ToInt32(method.Body.Instructions[i].Operand));
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    foreach (Instruction instr in instructions)
                    {
                        method.Body.Instructions.Insert(i + 1, instr);
                        i++;
                    }
                }
            }

            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                {
                    int operand = method.Body.Instructions[i].GetLdcI4Value();
                    if (operand <= 1) continue;
                    var two = NextInt(1, 10);
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = operand * two;
                    method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Ldc_I4, two));
                    method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Div));
                }
            }

            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                {
                    int operand = method.Body.Instructions[i].GetLdcI4Value();
                    if (operand <= 1) continue;
                    var two = NextInt(1, (int)((double)operand / 1.5));
                    var one = operand / two;
                    while (two * one != operand)
                    {
                        two = NextInt(1, (int)((double)operand / 1.5));
                        one = operand / two;
                    }
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = one;
                    method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Ldc_I4, two));
                    method.Body.Instructions.Insert(++i, new Instruction(OpCodes.Mul));
                }
            }
        }

        private static List<Instruction> Calc(int value)
        {
            List<Instruction> instructions = new List<Instruction>();
            int num = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100000);
            bool once = Convert.ToBoolean(new Random(Guid.NewGuid().GetHashCode()).Next(0, 2));
            int num1 = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100000);
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, value - num + (once ? (0 - num1) : num1)));
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num));
            instructions.Add(Instruction.Create(OpCodes.Add));
            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, num1));
            instructions.Add(Instruction.Create(once ? OpCodes.Add : OpCodes.Sub));
            return instructions;
        }

        public static int NextInt(int minValue, int maxValue)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            return (int)Math.Floor((double)(BitConverter.ToUInt32(b, 0) / uint.MaxValue) * (maxValue - minValue)) + minValue;
        }

        public static void Mutate6(MethodDef method)
        {
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                {
                    if (method.Body.Instructions[i].GetLdcI4Value() == 0)
                    {
                        switch (new Random().Next(0, 2))
                        {
                            case 0:
                                method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Add));
                                break;
                            case 1:
                                method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sub));
                                break;
                        }
                        method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldsfld, method.Module.Import(typeof(Type).GetField("EmptyTypes"))));
                        method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldlen));
                        i += 2;
                    }
                }
            }
        }

        public static void Mutate7(MethodDef method)
        {
            for (int i = 0; i < method.Body.Instructions.Count; i++)
            {
                if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                {
                    string value = method.Body.Instructions[i].Operand.ToString();
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(int).GetMethod("Parse", new Type[] { typeof(string) }))));
                    i += 2;
                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_S)
                {
                    string value = method.Body.Instructions[i].Operand.ToString();
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(short).GetMethod("Parse", new Type[] { typeof(string) }))));
                    i += 2;
                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I8)
                {
                    string value = method.Body.Instructions[i].Operand.ToString();
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(long).GetMethod("Parse", new Type[] { typeof(string) }))));
                    i += 2;
                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_R4)
                {
                    string value = method.Body.Instructions[i].Operand.ToString();
                    method.Body.Instructions[i].OpCode = OpCodes.Nop;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldstr, value));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Call, method.Module.Import(typeof(float).GetMethod("Parse", new Type[] { typeof(string) }))));
                    i += 2;
                }
            }
        }
    }
}