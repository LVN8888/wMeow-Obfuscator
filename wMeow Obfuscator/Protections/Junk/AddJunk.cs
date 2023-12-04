using dnlib.DotNet;
using Helper;
using System.Linq;
using System;

namespace Protections
{
    static class AddJunk
    {
        public static void Add(ModuleDef module)
        {
            int number = System.Convert.ToInt32(Random.Next(300,999));
            for (int i = 0; i < number; i++)
            {
                var junkattribute = new TypeDefUser(Utils.GenerateRandomString(), "Obfuscator", module.CorLibTypes.Object.TypeDefOrRef);
                module.Types.Add(junkattribute);
                var junkattribute2 = new TypeDefUser(RandomString(20)+RandomStringAndInt(10), "Obfuscator", module.CorLibTypes.Object.TypeDefOrRef);
                module.Types.Add(junkattribute2);
            }
        }
        public static Random Random = new Random();

        private static string RandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()-_+=[{}]|;:',<.>/?"
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
        private static string RandomStringAndInt(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
