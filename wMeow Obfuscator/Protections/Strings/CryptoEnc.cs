using System;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helper;

namespace Protections
{
    static class StringEncoder 
    {
        public static void Execute(ModuleDef M)
        {
            var module = ModuleDefMD.Load(typeof(Decoder).Module);
            var type = module.ResolveTypeDef(MDToken.ToRID(typeof(Decoder).MetadataToken));
            var decoderMethod = InjectHelper.Inject(type, M.GlobalType, M).SingleOrDefault() as MethodDef;

            var cryptoRandom = new xCryptoRandom();
            foreach (var typeDef in M.GetTypes().Where(x => x.HasMethods))
            {
                foreach (var methodDef in typeDef.Methods.Where(x => x.HasBody))
                {
                    var instructions = methodDef.Body.Instructions;
                    for (var i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode == OpCodes.Ldstr && !string.IsNullOrEmpty(instructions[i].Operand.ToString()))
                        {
                            var key = methodDef.Name.Length + cryptoRandom.Next();

                            var encryptedString = EncryptString(new Tuple<string, int>(instructions[i].Operand.ToString(), key));

                            instructions[i].OpCode = OpCodes.Ldstr;
                            instructions[i].Operand = encryptedString;
                            instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(key));
                            instructions.Insert(i + 2, OpCodes.Call.ToInstruction(decoderMethod));
                            i += 2;
                        }
                    }
                    methodDef.Body.SimplifyMacros(methodDef.Parameters);
                }
            }
        }
        private static string EncryptString(Tuple<string, int> values)
        {
            var stringBuilder = new StringBuilder();
            int key = values.Item2;
            foreach (var symbol in values.Item1)
                stringBuilder.Append((char) (symbol ^ key));
            return stringBuilder.ToString();
        }
    }
}