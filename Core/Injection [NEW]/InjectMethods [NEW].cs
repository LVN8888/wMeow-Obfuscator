using MeoxDLibHelper;
using MeoxDLibHelper.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Injection
{
    class InjectMethods
    {
        public static void methodInjector()
        {
            int pos = 0;
            foreach (Protection.MethodData methodData in Protection.MethodProccesor.AllMethods)
            {
                methodData.position = pos;
                var cipherLen = (methodData.DecryptedBytes.Length / 16 + 1) * 16;
                methodData.cipherSize = cipherLen;
                Injection.InjectInitialise.InjectMethod(methodData.position, methodData.ID, methodData.cipherSize, methodData.Method);
                pos += cipherLen;
            }
            /*ProxyResource.Execute(Protector.moduleDefMD);*/
            if (Protector.StringAnalysis != 0)
            {
                if (Protector.StringAnalysis == 1)
                {
                    MeoxDLibHelper.ReplaceObfuscator.Execute(Protector.moduleDefMD);
                }
                if (Protector.StringAnalysis == 2)
                {
                    MeoxDLibHelper.RemoveObfuscator.Execute(Protector.moduleDefMD);
                }
                if (Protector.StringAnalysis == 3)
                {
                    MeoxDLibHelper.ReplaceObfuscator.Execute(Protector.moduleDefMD);
                    MeoxDLibHelper.RemoveObfuscator.Execute(Protector.moduleDefMD);
                }
            }
            /*if (Protector.StringEncryption)
            {           
                //MeoxDLibHelper.StringEncryption.Execute(Protector.moduleDefMD);
            }*/
            /*if (Protector.Mutation)
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
            }*/
            
        }
    }
}
