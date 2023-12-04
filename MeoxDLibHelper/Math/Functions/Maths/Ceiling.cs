using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;

namespace MeoxDLibHelper
{
    public class Ceiling : iFunction
    {
        public override ArithmeticTypes ArithmeticTypes => ArithmeticTypes.Ceiling;

        public override ArithmeticVT Arithmetic(Instruction instruction, ModuleDef module)
        {
            if (!ArithmeticUtils.CheckArithmetic(instruction)) return null;
            var arithmeticTypes = new List<ArithmeticTypes> { ArithmeticTypes.Add, ArithmeticTypes.Sub };
            var arithmeticEmulator = new ArithmeticEmulator(instruction.GetLdcI4Value(), ArithmeticUtils.GetY(instruction.GetLdcI4Value()), ArithmeticTypes);
            return (new ArithmeticVT(new Value(arithmeticEmulator.GetValue(arithmeticTypes), arithmeticEmulator.GetY()), new Token(ArithmeticUtils.GetOpCode(arithmeticEmulator.GetType), module.Import(ArithmeticUtils.GetMethod(ArithmeticTypes))), ArithmeticTypes));
        }
    }
}