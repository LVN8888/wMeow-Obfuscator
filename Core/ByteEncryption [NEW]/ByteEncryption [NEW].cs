﻿using MeoxDLibHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.ByteEncryption
{
    class ByteEncryption
    {
        public static byte[] Encrypt(byte[] key, byte[] message)
        {
            using (var rijndael = new RijndaelManaged())
            {
                rijndael.KeySize = 256;
                rijndael.BlockSize = 128;
                rijndael.Key = key;
                rijndael.IV = key;
                return EncryptBytes(rijndael, message);
            }
        }
        private static byte[] EncryptBytes(
           SymmetricAlgorithm alg,
           byte[] message)
        {
            if (message == null || message.Length == 0)
                return message;

            if (alg == null)
                throw new ArgumentNullException("ALG is null");

            using (var stream = new MemoryStream())
            using (var encryptor = alg.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(message, 0, message.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }
        public static byte[] Encrypt2(byte[] data, byte[] Keys)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= Keys[i % Keys.Length];
            }
            return data;
        }
    }
}
