﻿using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protections
{
    internal class StringEncryptionBase64
    {
        public static void Run(ModuleDef md)
        {
            foreach (TypeDef type in md.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Body == null) continue;
                    method.Body.SimplifyBranches();
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            string base64toencrypt = method.Body.Instructions[i].Operand.ToString();
                            string base64EncryptedString = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(base64toencrypt));
                            method.Body.Instructions[i].OpCode = OpCodes.Nop;
                            method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, md.Import(typeof(System.Text.Encoding).GetMethod("get_UTF8", new Type[] { }))));
                            method.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Ldstr, base64EncryptedString));
                            method.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Call, md.Import(typeof(System.Convert).GetMethod("FromBase64String", new Type[] { typeof(string) }))));
                            method.Body.Instructions.Insert(i + 4, new Instruction(OpCodes.Callvirt, md.Import(typeof(System.Text.Encoding).GetMethod("GetString", new Type[] { typeof(byte[]) }))));
                            i += 4;
                        }
                    }
                }
            }
        }
    }
}
