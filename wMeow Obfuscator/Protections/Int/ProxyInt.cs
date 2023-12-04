using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protections
{
    internal class ProxyInt
    {
        public static void Execute(ModuleDef module)
        {
            foreach (var type in module.GetTypes())
            {
                if (type.IsGlobalModuleType) continue;
                List<MethodDef> methodsToAdd = new List<MethodDef>(); // Tạo danh sách phương thức phụ
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    if (method.Name == "Dispose") continue;
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {                       
                        if (method.Body.Instructions[i].IsLdcI4())
                        {
                            var methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
                            var methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
                            var meth1 = new MethodDefUser(RandomString(20) + RandomInt(10),
                                        MethodSig.CreateStatic(module.CorLibTypes.Int32),
                                        methImplFlags, methFlags);
                            //module.GlobalType.Methods.Add(meth1);
                            meth1.Body = new CilBody();
                            meth1.Body.Variables.Add(new Local(module.CorLibTypes.Int32));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, instr[i].GetLdcI4Value()));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                            methodsToAdd.Add(meth1); // Thêm phương thức vào danh sách phụ
                            instr[i].OpCode = OpCodes.Call;
                            instr[i].Operand = meth1;
                        }
                        else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_R4)
                        {
                            var methImplFlags = MethodImplAttributes.IL | MethodImplAttributes.Managed;
                            var methFlags = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot;
                            var meth1 = new MethodDefUser(RandomString(20)+ RandomInt(10),
                                        MethodSig.CreateStatic(module.CorLibTypes.Double),
                                        methImplFlags, methFlags);
                            //module.GlobalType.Methods.Add(meth1);
                            meth1.Body = new CilBody();
                            meth1.Body.Variables.Add(new Local(module.CorLibTypes.Double));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_R4, (float)method.Body.Instructions[i].Operand));
                            meth1.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                            methodsToAdd.Add(meth1); // Thêm phương thức vào danh sách phụ
                            instr[i].OpCode = OpCodes.Call;
                            instr[i].Operand = meth1;
                        }
                    }
                }
                foreach (var methodToAdd in methodsToAdd)
                {
                    type.Methods.Add(methodToAdd); // Thêm các phương thức từ danh sách phụ vào danh sách chính
                }
            }
        }
        public static Random Random = new Random();

        private static string RandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private static string RandomInt(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
