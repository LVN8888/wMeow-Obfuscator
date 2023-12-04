using System.Collections.Generic;
using System.Windows.Forms;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Helper;
using MeoxDLibHelper;

namespace Protections
{
    internal class EConstants
    {
        public void Inject()
        {
            TypeDef tDef = this.typeDef("Runtime.CRuntime");
            ModuleDef module = MLib.MeoLibrary.assembly.ManifestModule.Types[0].Module;
            MethodDef methodDef = this.methodDef(tDef, "CRT");
            methodDef.DeclaringType = MLib.MeoLibrary.assembly.ManifestModule.Types[0];
            methodDef.Name = Helper.Utils.GenerateRandomString();
            InjectHelper.Inject(methodDef, module);
            TypeDef tDef2 = this.typeDef("Runtime.CRuntime");
            ModuleDef module2 = MLib.MeoLibrary.assembly.ManifestModule.Types[0].Module;
            MethodDef methodDef2 = this.methodDef(tDef2, "CRT");
            methodDef2.DeclaringType = methodDef.DeclaringType;
            methodDef2.Name = Helper.Utils.GenerateRandomString();
            InjectHelper.Inject(methodDef2, module2);
            List<string> list = new List<string>();
            for (int i = 0; i < MLib.MeoLibrary.assembly.Modules.Count; i++)
            {
                ModuleDef moduleDef = MLib.MeoLibrary.assembly.Modules[i];
                for (int j = 0; j < moduleDef.Types.Count; j++)
                {
                    TypeDef typeDef = moduleDef.Types[j];
                    for (int k = 0; k < typeDef.Methods.Count; k++)
                    {
                        MethodDef methodDef3 = typeDef.Methods[k];
                        if (!methodDef3.HasBody)
                        {
                            return;
                        }
                        int count = methodDef3.Body.Instructions.Count;
                        for (int l = 0; l < count; l++)
                        {
                            Instruction instruction = methodDef3.Body.Instructions[l];
                            string text = Helper.Utils.GenerateRandomString();
                            if (instruction.OpCode == OpCodes.Ldstr && !list.Contains(instruction.Operand.ToString()))
                            {
                                int num = Helper.Utils.GetInt();
                                instruction.Operand = Runtime.CRuntime.CRT(instruction.Operand.ToString(), text);
                                methodDef3.Body.Instructions.Insert(l + 1, new Instruction(OpCodes.Ldstr, text));
                                methodDef3.Body.Instructions.Insert(l + 2, new Instruction(OpCodes.Call, (num == 0) ? methodDef : methodDef2));
                                methodDef3.Body.OptimizeBranches();
                                methodDef3.Body.SimplifyBranches();
                                list.Add(text);
                            }
                        }
                    }
                }
            }
        }
        private TypeDef typeDef(string name)
        {
            AssemblyDef assemblyDef = AssemblyDef.Load(Application.ExecutablePath);
            return assemblyDef.ManifestModule.Find(name, false);
        }
        private MethodDef methodDef(TypeDef tDef, string name)
        {
            return tDef.FindMethod(name);
        }
    }
}
