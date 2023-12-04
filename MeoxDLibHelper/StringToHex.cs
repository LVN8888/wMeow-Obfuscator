using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Helper;

namespace MeoxDLibHelper
{
    public class StringToHex
    {
        public static void Execute(ModuleDefMD module)
        {
            var moduletwo = ModuleDefMD.Load(typeof(HexString).Module);
            var typeDef = moduletwo.ResolveTypeDef(MDToken.ToRID(typeof(HexString).MetadataToken));
            IEnumerable<IDnlibDef> source = InjectHelper.Inject(typeDef, module.GlobalType, module);
            MethodDef method2 = (MethodDef)source.Single((IDnlibDef method) => method.Name == "Hex2String");

            foreach (TypeDef type in module.Types)
            {
                if (type.IsGlobalModuleType)
                    continue;
                if (type.Namespace == "Costura")
                    continue;
                foreach (MethodDef method in type.Methods)
                {
                    if (method.HasBody)
                    {
                        var instr = method.Body.Instructions;
                        method.Body.SimplifyBranches();
                        for (var i = 0; i < instr.Count; i++)
                        {
                            if (instr[i].OpCode == OpCodes.Ldstr)
                            {
                                string oldString = method.Body.Instructions[i].Operand.ToString();
                                method.Body.Instructions[i].Operand = HexString.StringHex(oldString, true); //true = with space // false = without space !
                                method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Ldnull));
                                method.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Call, method2));
                                i += 2;
                            }
                        }
                        instr.OptimizeBranches();
                    }
                }
            }
        }
        
    }
    public static class HexString
    {
        public static string StringHex(string str, bool space)
        {
            if (space)
                return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", " ");
            else
                return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", "");
        }
        public static string Hex2String(string mHex, object obj)
        {
            obj = 0;
            mHex = Regex.Replace(mHex, "[^0-9A-Fa-f]", "");
            if (mHex.Length % 2 != Convert.ToInt32(obj))
                mHex = mHex.Remove(mHex.Length - 1, 1);
            if (mHex.Length <= 0) return "";
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return Encoding.Default.GetString(vBytes);
        }
    }
}
