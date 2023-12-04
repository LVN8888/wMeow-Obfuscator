using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System.Linq;
using System.Text;
using TypeAttributes = dnlib.DotNet.TypeAttributes;
using Helper;

namespace MeoxDLibHelper
{
    public class StringEncryptionASCII
    {
        public static void Execute(ModuleDef moduleDef)
        {    
            var module = ModuleDefMD.Load(typeof(MeoxxD).Module);
            var type1 = module.ResolveTypeDef(MDToken.ToRID(typeof(MeoxxD).MetadataToken));
            TypeDef panda = new TypeDefUser("MeoxxD", moduleDef.CorLibTypes.Object.TypeDefOrRef);
            panda.Attributes = TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.AnsiClass;
            moduleDef.Types.Add(panda);
            var decoderMethodmembers = InjectHelper.Inject(type1, panda, moduleDef).SingleOrDefault() as MethodDef;
            foreach (TypeDef type in moduleDef.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Name == "InitializeComponent") continue;
                    if (method.Body == null) continue;
                    for (int i = 0; i < method.Body.Instructions.Count(); i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            string oldString = method.Body.Instructions[i].Operand.ToString();
                            string newString = EncryptString(oldString);
                            method.Body.Instructions[i].OpCode = OpCodes.Nop;
                            method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Ldstr, newString));
                            method.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Call, decoderMethodmembers));
                            i += 2;
                        }
                        method.Body.OptimizeBranches();
                        method.Body.SimplifyBranches();
                    }
                }
            }
        }


        private static string EncryptString(string str)
        {
            byte[] byteString = Encoding.ASCII.GetBytes(str);
            for (var i = 0; i < byteString.Length; i++)
            {
                byteString[i] += 1;
            }
            return Encoding.ASCII.GetString(byteString);
        }
    }
    internal static class MeoxxD
    {
        public static string MeoxD(string encodedString)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(encodedString);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] -= 1;
            }
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
