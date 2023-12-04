using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace MeoxDLibHelper
{
    public abstract class iFunction
    {
        public abstract ArithmeticTypes ArithmeticTypes { get; }

        public abstract ArithmeticVT Arithmetic(Instruction instruction, ModuleDef module);
    }
}