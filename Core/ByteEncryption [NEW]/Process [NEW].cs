using Core.Protection;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Core.ByteEncryption
{
    class Process
    {
        public static byte[] tester(MethodDef methodDef, ModuleDefMD updated)
        {
            dnlib.IO.IImageStream streamFull = updated.MetaData.PEImage.CreateFullStream();
            var upated = (updated.ResolveToken(methodDef.MDToken.ToInt32()) as MethodDef);
            var offset = updated.MetaData.PEImage.ToFileOffset(upated.RVA);
            streamFull.Position = (long)offset;
            byte b = streamFull.ReadByte();

            ushort flags;
            byte headerSize;
            ushort maxStack;
            uint codeSize = 0;

            switch (b & 7)
            {
                case 2:
                case 6:
                    flags = 2;
                    maxStack = 8;
                    codeSize = (uint)(b >> 2);
                    headerSize = 1;
                    break;

                case 3:
                    flags = (ushort)((streamFull.ReadByte() << 8) | b);
                    headerSize = (byte)(flags >> 12);
                    maxStack = streamFull.ReadUInt16();
                    codeSize = streamFull.ReadUInt32();
                    break;
            }
            if (codeSize != 0)
            {
                byte[] il_byte = new byte[codeSize];
                streamFull.Position = (long)offset + upated.Body.HeaderSize;
                streamFull.Read(il_byte, 0, il_byte.Length);
                return il_byte;
            }
            return null;
        }

        public unsafe static void processConvertedMethods(List<MethodData> allMethodDatas)
        {
            int pos = 0;
            Stream tester = new MemoryStream();
            ModuleWriterOptions modopts = new ModuleWriterOptions(Protector.moduleDefMD);
            modopts.MetaDataOptions.Flags = MetaDataFlags.PreserveAll;
            modopts.Logger = DummyLogger.NoThrowInstance;
            Protector.moduleDefMD.Write(tester, modopts);
            ModuleDefMD updated = ModuleDefMD.Load(tester);

            foreach (MethodData methodData in allMethodDatas)
            {
                var decryptedBytes = methodData.DecryptedBytes;
                var method = methodData.Method;
                byte[] methodBytes = Process.tester(method, updated);
                var enc = ByteEncryption.Encrypt(ByteEncryption.Encrypt2(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(method.Name)), Encoding.ASCII.GetBytes("Bytes")), decryptedBytes);

                enc = aMethod2(enc, enc.Length, methodBytes, methodBytes.Length);
                methodData.EncryptedBytes = enc;

                methodData.Encrypted = true;
                methodData.size = methodData.EncryptedBytes.Length;
                methodData.position = pos;
                pos += methodData.EncryptedBytes.Length;
                //////////////////////////////////////////////////////////
            }
        }

        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private static byte[] b(byte[] toEncrypt, int len)
        {
            string key = "HCP"; //Any chars will work, in an array of any size

            byte[] output = toEncrypt;

            for (int i = 0; i < len; i++)
            {
                //C++ TO C# CONVERTER WARNING: This 'sizeof' ratio was replaced with a direct reference to the array length:
                //ORIGINAL LINE: output[i] = toEncrypt[i] ^ key[i % (sizeof(key) / sizeof(sbyte))];
                output[i] = (byte)(toEncrypt[i] ^ key[i % (key.Length)]);
            }

            return output;
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        //C++ TO C# CONVERTER NOTE: __stdcall is not available in C#:
        //ORIGINAL LINE: EXTERN_DLL_EXPORT void __stdcall a(byte * data, int datalen, byte key[], int keylen)
        private static byte[] aMethod2(byte[] data, int datalen, byte[] key, int keylen)
        {
            int N1 = 12;
            int N2 = 14;
            int NS = 258;
            int I = 0;
            for (I = 0; I < keylen; I++)
            {
                NS += NS % (key[I] + 1);
            }

            for (I = 0; I < datalen; I++)
            {
                NS = key[I % keylen] + NS;
                N1 = (NS + 5) * (N1 & 255) + (N1 >> 8);
                N2 = (NS + 7) * (N2 & 255) + (N2 >> 8);
                NS = ((N1 << 8) + N2) & 255;

                data[I] = (byte)((data[I]) ^ NS);
            }
            return b(data, datalen);

        }
    }
}
