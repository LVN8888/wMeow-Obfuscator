using dnlib.DotNet;

namespace Protections
{
    public class AntiILDasm
    {
        public static void ildasm(ModuleDef module)
        {
            TypeRef attrRef = module.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute");
            var ctorRef = new MemberRefUser(module, ".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), attrRef);
            var attr = new CustomAttribute(ctorRef);
            module.CustomAttributes.Add(attr);
        }
    }
}