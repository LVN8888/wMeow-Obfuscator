using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using OpCodes = dnlib.DotNet.Emit.OpCodes;

namespace MeoxDLibHelper
{
    public class Mutations
    {
        private static Random random = new Random(DateTime.Now.Millisecond);

        public static void Execute(ModuleDef module)
        {
            foreach (TypeDef type in (from types in module.Types where types.HasMethods && !types.IsGlobalModuleType select types).ToArray<TypeDef>())
            {
                if (type.Name == "<Module>") continue;
                foreach (MethodDef method in (from methods in type.Methods where methods.HasBody && methods.Body.HasInstructions && methods.Body.Instructions.Count > 1 && !methods.DeclaringType.IsGlobalModuleType select methods).ToArray<MethodDef>())
                {
                    if (method.Name != "Main") continue;
                    /*if (method.Name == "ADum") continue;
                    if (method.Name == "InitializeComponent") continue;
                    if (method.Name == "Dispose") continue;*/
                    
                    method.Body.SimplifyMacros(method.Parameters);
                    method.Body.SimplifyBranches();
                    for (int oo = 0; oo < 1; oo++)
                        for (int i = 0; i < method.Body.Instructions.Count; i++)
                        {
                            if ((method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4
                                   || method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_S) && CanProtect(method.Body.Instructions, i))
                            {
                                int fork = 0;
                                switch (random.Next(0, 2))
                                {
                                    case 0:
                                        CreateOperation(method, i, ref fork);
                                        break;
                                    case 1:
                                        CreateShortIf(method, i, ref fork);
                                        break;
                                }
                                i += fork;
                            }
                        }
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4 && CanProtect(method.Body.Instructions, i))
                        {
                            int fork = 0;
                            if (random.Next(0, 101) < 80)
                                LocalMaker((ModuleDefMD)module, method, i, ref fork);
                            i += fork;
                        }
                    }
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
                    //Mutate7(method);
                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                    method.Body.SimplifyMacros(method.Parameters);
                    method.Body.OptimizeMacros();
                }
            }
        }
        public static void Execute2(ModuleDef module)
        {
            foreach (TypeDef type in (from types in module.Types where types.HasMethods && !types.IsGlobalModuleType select types).ToArray<TypeDef>())
            {
                if (type.Name == "<Module>") continue;
                foreach (MethodDef method in (from methods in type.Methods where methods.HasBody && methods.Body.HasInstructions && methods.Body.Instructions.Count > 1 && !methods.DeclaringType.IsGlobalModuleType select methods).ToArray<MethodDef>())
                {
                    Mutate7(method);
                    method.Body.SimplifyMacros(method.Parameters);
                    method.Body.SimplifyBranches();
                    
                    
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
                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                    method.Body.SimplifyMacros(method.Parameters);
                    method.Body.OptimizeMacros();
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
        private static void LocalMaker(ModuleDefMD module, MethodDef method, int i, ref int fork)
        {
            Local local = new Local(method.Module.CorLibTypes.Int32);
            method.Body.Variables.Add(local);
            int operand = method.Body.Instructions[i].GetLdcI4Value();
            method.Body.Instructions[i].OpCode = OpCodes.Ldloc;
            method.Body.Instructions[i].Operand = local;

            TypeDefUser structDef2 = new TypeDefUser(Utils.Rename(3), module.CorLibTypes.GetTypeRef("System", "ValueType"));
            structDef2.ClassLayout = new ClassLayoutUser(1, 0);
            structDef2.Attributes |= TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.Public;

            int ints4Count = random.Next(0, 4);
            int ints1Count = random.Next(1, 5);
            int ints2Count = random.Next(0, 4);
            int ints8Count = random.Next(1, 4);

            for (int j = 0; j < ints4Count; j++)
                structDef2.Fields.Add(new FieldDefUser(Utils.Rename(1), new FieldSig(module.Import(typeof(Int32)).ToTypeSig()), FieldAttributes.Public));
            for (int j = 0; j < ints2Count; j++)
                structDef2.Fields.Add(new FieldDefUser(Utils.Rename(1), new FieldSig(module.Import(typeof(Int16)).ToTypeSig()), FieldAttributes.Public));
            for (int j = 0; j < ints8Count; j++)
                structDef2.Fields.Add(new FieldDefUser(Utils.Rename(1), new FieldSig(module.Import(typeof(Int64)).ToTypeSig()), FieldAttributes.Public));
            for (int j = 0; j < ints1Count; j++)
                structDef2.Fields.Add(new FieldDefUser(Utils.Rename(1), new FieldSig(module.Import(typeof(byte)).ToTypeSig()), FieldAttributes.Public));

            method.Module.Types.Add(structDef2);

            switch (random.Next(0, 4))
            {
                case 0:
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldc_I4, operand + (8 * ints8Count) + (4 * ints4Count) + (2 * ints2Count) + (1 * ints1Count)));
                    method.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Sizeof, structDef2));
                    method.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Sub));
                    method.Body.Instructions.Insert(3, Instruction.Create(OpCodes.Stloc, local));
                    fork = 5;
                    break;

                case 1:
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldc_I4, operand - (8 * ints8Count) - (4 * ints4Count) - (2 * ints2Count) - (1 * ints1Count)));
                    method.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Sizeof, structDef2));
                    method.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Add));
                    method.Body.Instructions.Insert(3, Instruction.Create(OpCodes.Stloc, local));
                    fork = 5;
                    break;

                case 2:
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldc_I4, operand + (8 * ints8Count) + (4 * ints4Count) + (2 * ints2Count) + (1 * ints1Count)));
                    method.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Sizeof, structDef2));
                    method.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Sub));
                    method.Body.Instructions.Insert(3, Instruction.Create(OpCodes.Stloc, local));
                    fork = 3;
                    break;

                case 3:
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldc_I4, operand - (8 * ints8Count) - (4 * ints4Count) - (2 * ints2Count) - (1 * ints1Count)));
                    method.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Sizeof, structDef2));
                    method.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Add));
                    method.Body.Instructions.Insert(3, Instruction.Create(OpCodes.Stloc, local));
                    fork = 3;
                    break;
            }
        }

        private static void CreateOperation(MethodDef method, int i, ref int fork)
        {
            int number = method.Body.Instructions[i].GetLdcI4Value();
            int RDMD = random.Next(1, 10);
            int RD = random.Next(short.MinValue, short.MaxValue);
            bool Special = Convert.ToBoolean(random.Next(0, 2));
            bool True = Convert.ToBoolean(random.Next(0, 2));
            switch (random.Next(0, 6))
            {
                case 0:
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = number - RD;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, RD));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(True ? OpCodes.Add : OpCodes.Add_Ovf));
                    fork = 2;
                    break;

                case 1:
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = number + RD;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, RD));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(True ? OpCodes.Sub : OpCodes.Sub_Ovf));
                    fork = 2;
                    break;

                case 2:
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = number ^ RD;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, RD));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Xor));
                    fork = 2;
                    break;

                case 3:
                    int rand1 = random.Next(1, 10000);
                    int rand2 = random.Next(1, 10000);
                    int rest = rand1 % rand2;
                    int Original_Operand = (int)method.Body.Instructions[i].GetLdcI4Value();
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = Original_Operand - rest;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, rand1));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, rand2));
                    method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Rem));
                    method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Add));
                    fork = 4;
                    break;

                case 4:
                    int Original_Operands = (int)method.Body.Instructions[i].GetLdcI4Value();
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = Original_Operands;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, Original_Operands));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Or));
                    fork = 2;
                    break;

                case 5:
                    int rand3 = random.Next(10000);
                    int rand4 = random.Next(10000);
                    int reste = rand3 >> rand4;
                    int Original_Operand_4 = (int)method.Body.Instructions[i].GetLdcI4Value();
                    method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
                    method.Body.Instructions[i].Operand = Original_Operand_4 + reste;
                    method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, rand3));
                    method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, rand4));
                    method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Shr));
                    method.Body.Instructions.Insert(i + 4, Instruction.Create(True ? OpCodes.Sub : OpCodes.Sub_Ovf));
                    fork = 4;
                    break;

                default: break;
            }
        }
        private static void CreateShortIf(MethodDef method, int i, ref int fork)
        {
            int number = (int)method.Body.Instructions[i].GetLdcI4Value();
            int numberfake = number / 2;
            int random_1 = random.Next();
            int random_2 = random.Next();
            bool islower = (random_1 < random_2);
            method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
            method.Body.Instructions[i].Operand = random_1;
            method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Ldc_I4, random_2));
            method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Nop));//BLT.S
            method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, islower ? numberfake : number));
            method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Nop));//BR.S
            method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Ldc_I4, islower ? number : numberfake));


            method.Body.Instructions[i + 2].OpCode = OpCodes.Blt_S;
            method.Body.Instructions[i + 2].Operand = method.Body.Instructions[i + 5];
            method.Body.Instructions[i + 4].OpCode = OpCodes.Br_S;
            method.Body.Instructions[i + 4].Operand = method.Body.Instructions[i + 6];

            fork = 6;
        }
        private static bool CanProtect(IList<Instruction> instructions, int i)
        {
            if (instructions[i + 1].GetOperand() != null)
            {
                if (instructions[i + 1].Operand.ToString().Contains("bool"))
                {
                    return false;
                }
            }
            if (instructions[i + 1].OpCode == OpCodes.Newobj)
            {
                return false;
            }
            if (instructions[i].GetLdcI4Value() == 0 || instructions[i].GetLdcI4Value() == 1)
            {
                return false;
            }
            return true;
        }
    }
}
