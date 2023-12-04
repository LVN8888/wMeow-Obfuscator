using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.PE;
using Core.Injection;

namespace Core.Protection
{
    internal class MethodProccesor
    {
        public static List<MethodData> AllMethods = new List<MethodData>();
        public static void ModuleProcessor()
        {
            int value = 0;
            foreach (var typeDef in Protector.moduleDefMD.GetTypes())
            {
                if (typeDef == Protector.moduleDefMD.GlobalType) continue;
                if (typeDef.HasGenericParameters) continue;
                if (typeDef.CustomAttributes.Count(i => i.TypeFullName.Contains("CompilerGenerated")) != 0) continue;
                if (typeDef.IsValueType) continue;
                foreach (var method in typeDef.Methods)
                {
                    //if (method.MDToken.ToInt32() != 0x0600017E) continue;
                    //	if (Protector.moduleDefMD.EntryPoint != method) continue;
                    if (Protector.usedMethodsFullNames.Contains(method.FullName) && !method.IsConstructor && method.HasBody && (!typeDef.IsGlobalModuleType || !method.IsConstructor) && !method.HasGenericParameters)
                    {
                        if (method.IsConstructor) continue;
                        if (method.DeclaringType.IsGlobalModuleType) continue;
                        if (!method.HasBody) continue;
                        if (typeDef.IsGlobalModuleType && method.IsConstructor) continue;
                        if (method.HasGenericParameters) continue;
                        if (method.CustomAttributes.Count(i => i.TypeFullName.Contains("CompilerGenerated")) != 0) continue;
                        if (method.ReturnType == null) continue;
                        if (method.ReturnType.IsGenericParameter) continue;
                        if (method.Parameters.Count(i => i.Type.FullName.EndsWith("&") && i.ParamDef.IsOut == false) != 0) continue;
                        if (method.CustomAttributes.Count(i => i.NamedArguments.Count == 2 && i.NamedArguments[0].Value.ToString().Contains("Encrypt") &&
                                                                i.NamedArguments[1].Name.Contains("Exclude") && i.NamedArguments[1].Value
                                                                 .ToString().ToLower().Contains("true")) != 0) continue;
                        MethodData methodData = new MethodData(method);//create instance of custom class

                        method.Body.SimplifyMacros(method.Parameters);
                        method.Body.SimplifyBranches();
                        var convertor = new ConvertToBytes(method);
                        try
                        {
                            convertor.ConversionMethod();//we convert our method to byte array

                            if (!convertor.Successful) continue;//only carry on if the conversion was successful
                            methodData.Converted = true;//set conversion to true 
                            methodData.DecryptedBytes = convertor.ConvertedBytes;//set methodData bytes to the coverted bytes
                            methodData.ID = value;//set the methodID
                            AllMethods.Add(methodData);
                            value++;//increase value which is methodID
                        }
                        catch
                        {
                        }
                    }            
                }
            }

            

            Injection.InjectInitialise.injectIntoCctor();//inject the setup methods into the module cctor which is the very first method that is executed in the .net module
            
            Injection.InjectMethods.methodInjector();//inject the methods to remove the old code and add the call to the decryption
                                                     //if ((Protector.moduleDefMD.Characteristics & Characteristics.Dll) == 0)
                                                     //{
                                                     //	var vody = Protector.moduleDefMD.EntryPoint.Body;
                                                     //	vody.Instructions.Insert(0, new Instruction(OpCodes.Ldstr, "TestResc"));
                                                     //	vody.Instructions.Insert(1, new Instruction(OpCodes.Call, InjectInitialise.conversionInit));
                                                     //}

            if ((Protector.moduleDefMD.Characteristics & Characteristics.Dll) == 0)
            {
                bool set = false;
                var vody = Protector.moduleDefMD.GlobalType.FindOrCreateStaticConstructor().Body;
                for (int i = 1; i < vody.Instructions.Count; i++)
                {
                    if (vody.Instructions[i].OpCode == OpCodes.Call)
                    {
                        MethodDef method = (MethodDef)vody.Instructions[i].Operand;
                        method.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldnull));
                        method.Body.Instructions.Insert(1, new Instruction(OpCodes.Ldstr, Str("+!$"))); //+!$%#
                        method.Body.Instructions.Insert(2, new Instruction(OpCodes.Ldstr, "d01lb3c="));                                          
                        method.Body.Instructions.Insert(3, new Instruction(OpCodes.Ldstr, Str("MeoxD^!"))); //MeoxD^!
                        method.Body.Instructions.Insert(4, new Instruction(OpCodes.Call, InjectInitialise.conversionInit));
                        set = true;
                        break;
                    }
                }
                if (set == false)
                {
                    var vody2 = Protector.moduleDefMD.EntryPoint.Body;
                    vody2.Instructions.Insert(0, new Instruction(OpCodes.Ldnull));
                    vody2.Instructions.Insert(1, new Instruction(OpCodes.Ldstr, MethodProccesor.Str("+!$"))); //+!$%#
                    vody2.Instructions.Insert(2, new Instruction(OpCodes.Ldstr, "d01lb3c="));
                    vody2.Instructions.Insert(3, new Instruction(OpCodes.Ldstr, Str("MeoxD^!"))); //MeoxD^!
                    vody2.Instructions.Insert(4, new Instruction(OpCodes.Call, InjectInitialise.conversionInit));
                }
            }
            
            if (Protector.StringEncryption)
            {
                foreach (var type in Protector.moduleDefMD.GetTypes())
                {
                    //if (type.Name != "Program" || type.Name != "<Module>") continue;
                    foreach (var method in type.Methods)
                    {
                        if (method.Name == "ADum" || method.Name == "Main")
                        {
                            MeoxDLibHelper.StringEncryption.ExecuteMethod(method);
                        }                       
                    }
                }

            }
            if (Protector.NumObf)
            {
                foreach (Protection.MethodData methodData in Protection.MethodProccesor.AllMethods)
                {
                    MeoxDLibHelper.NumObfuscation.ExecuteMethod(methodData.Method);
                }
            }
            if (Protector.Mutation)
            {
                MeoxDLibHelper.Mutations.Execute(Protector.moduleDefMD);
            }
            if (Protector.NumObf)
            {
                foreach (Protection.MethodData methodData in Protection.MethodProccesor.AllMethods)
                {
                    MeoxDLibHelper.NumObfuscation.ExecuteMethod(methodData.Method);
                }
            }
            if (Protector.MathInt)
            {
                MeoxDLibHelper.Arithmetic.Execute(Protector.moduleDefMD);
            }

            ByteEncryption.Process.processConvertedMethods(AllMethods);
            List<byte> allBytes = new List<byte>();
            foreach (var meth in AllMethods)
            {
                allBytes.AddRange(meth.EncryptedBytes);//add all bytes of all methods into one byte array
            }
            byte[] bytesName = Encoding.ASCII.GetBytes(Protector.name);
            bytesName = ByteEncryption.ByteEncryption.Encrypt(new byte[] { 0xDD, 0xFF, 0x15, 0x53, 0xa2, 0x65, 0x90, 0x12, 0x00, 0xaa, 12, 54, 66, 34, 23, 65 }, bytesName);
            allBytes.AddRange(bytesName);

            EmbeddedResource emb = new EmbeddedResource("+!$%#", exclusiveOR(allBytes.ToArray(),1));//create an embededd resource which we add to module later

            Protector.moduleDefMD.Resources.Add(emb);//add to module
            
        }
        public static string Str(string text)
        {
            var result = new StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)"^Meow^/"[c % "^Meow^/".Length]));

            return result.ToString();
        }
        public static byte[] exclusiveOR(byte[] arr1, int io)
        {

            Random rand = new Random(23546654);

            byte[] result = new byte[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                result[i] = (byte)(arr1[i] ^ (rand.Next(0, 100) + io));
                //result[i] = (byte)(arr1[i] ^ rand.Next(0, 100));
            }


            return result;
        }
    }
}