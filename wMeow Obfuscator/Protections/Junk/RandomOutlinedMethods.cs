using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Linq;
using Helper;
using System;

namespace Protections
{
    public static class RandomOutlinedMethods
    {
        public static void Add(ModuleDef module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods.ToArray())
                {
                    MethodDef strings = CreateReturnMethodDef(Utils.GenerateRandomString(), method);
                    MethodDef ints = CreateReturnMethodDef(Utils.GenerateRandomString(), method);
                    type.Methods.Add(strings);
                    type.Methods.Add(ints);
                }
            }
        }
        public static void AddVoidMethod(ModuleDef module)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods.ToArray())
                {
                    MethodSig methodSig = MethodSig.CreateStatic(module.CorLibTypes.Void);
                    MethodDef newMethod = new MethodDefUser("MethodName", methodSig, (MethodImplAttributes)(MethodAttributes.Public | MethodAttributes.Static));

                    // Thêm phương thức mới vào lớp
                    type.Methods.Add(newMethod);
                }
            }
            
        }

        static MethodDef CreateReturnMethodDef(object value, MethodDef source_method)
        {
            CorLibTypeSig corlib = null;

            if (value is int)
                corlib = source_method.Module.CorLibTypes.Int32;
            else if (value is float)
                corlib = source_method.Module.CorLibTypes.Single;
            else if (value is string)
                corlib = source_method.Module.CorLibTypes.String;
            MethodDef newMethod = new MethodDefUser(RandomString(20)+RandomStringAndInt(10),
                    MethodSig.CreateStatic(corlib),
                    MethodImplAttributes.IL | MethodImplAttributes.Managed,
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
            {
                Body = new CilBody()
            };
            if (value is int)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)value));
            else if (value is float)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_R4, (double)value));
            else if (value is string)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, (string)value));
            newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
            return newMethod;
        }

        public static Random Random = new Random();

        private static string RandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()-_+=[{}]|;:',<.>/?"
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
        private static string RandomStringAndInt(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}