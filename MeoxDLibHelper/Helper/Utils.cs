using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MeoxDLibHelper
{
    public static class Utils
    {
        private static List<string> used_names = new List<string>();
        public static string Rename(int a)
        {
            string renamer = null;
            switch (a)
            {
                case 0:
                    renamer = Guid.NewGuid().ToString().ToUpper().Substring(0, 8);
                    break;

                case 1:
                    renamer = Guid.NewGuid().ToString().ToUpper().Replace("-", string.Empty);
                    break;

                case 2:
                    IncrementNameId();
                    renamer = EncodeString(nameId, reflectionCharset);
                    break;

                case 3:
                    byte[] buff2 = SHA1.Create().ComputeHash(Encoding.Unicode.GetBytes(Guid.NewGuid().ToString())).Take(4).ToArray<byte>();
                    renamer = EncodeString(buff2, unicodeCharset);
                    break;
            }
            return renamer;
        }
        #region Charsets

        static readonly char[] asciiCharset = Enumerable.Range(32, 95)
            .Select(ord => (char)ord)
            .Except(new[] { '.' })
            .ToArray();

        static readonly char[] reflectionCharset = asciiCharset.Except(new[] { ' ', '[', ']' }).ToArray();

        static readonly char[] letterCharset = Enumerable.Range(0, 26)
            .SelectMany(ord => new[] { (char)('a' + ord), (char)('A' + ord) })
            .ToArray();

        static readonly char[] alphaNumCharset = Enumerable.Range(0, 26)
            .SelectMany(ord => new[] { (char)('a' + ord), (char)('A' + ord) })
            .Concat(Enumerable.Range(0, 10).Select(ord => (char)('0' + ord)))
            .ToArray();

        // Especially chosen, just to mess with people.
        // Inspired by: http://xkcd.com/1137/ :D
        static readonly char[] unicodeCharset = new char[] { }
            .Concat(Enumerable.Range(0x200b, 5).Select(ord => (char)ord))
            .Concat(Enumerable.Range(0x2029, 6).Select(ord => (char)ord))
            .Concat(Enumerable.Range(0x206a, 6).Select(ord => (char)ord))
            .Except(new[] { '\u2029' })
            .ToArray();

        #endregion

        static readonly byte[] nameId = new byte[8];

        private static void IncrementNameId()
        {
            for (int i = nameId.Length - 1; i >= 0; i--)
            {
                nameId[i]++;
                if (nameId[i] != 0)
                    break;
            }
        }
        private static string EncodeString(byte[] buff, char[] charset)
        {
            int i = (int)buff[0];
            StringBuilder stringBuilder = new StringBuilder();
            for (int j = 1; j < buff.Length; j++)
            {
                for (i = (i << 8) + (int)buff[j]; i >= charset.Length; i /= charset.Length)
                {
                    stringBuilder.Append(charset[i % charset.Length]);
                }
            }
            if (i != 0)
            {
                stringBuilder.Append(charset[i % charset.Length]);
            }
            return stringBuilder.ToString();
        }
        public static string GenerateRandomString()
        {
            string randomString_md5;

            do
            {
                Random rnd = new Random();

                string randomString = GenerateRandomString(rnd.Next(2, 24));

                randomString_md5 = MD5Hash(randomString);

                if (char.IsDigit(randomString_md5[0]))
                {
                    char randomLetter = GetLetter();

                    randomString_md5 = randomString_md5.Replace(randomString_md5[0], randomLetter);
                }
            } while (CheckStringExists(randomString_md5));

            used_names.Add(randomString_md5);

            return randomString_md5;
        }

        private static string GenerateRandomString(int size)
        {
            var charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var chars = charSet.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        private static char GetLetter()
        {
            Random rnd = new Random();

            int num = rnd.Next(0, 25);

            return (char)('a' + num);
        }
        public static int GetInt()
        {
            Random rnd = new Random();

            int num = rnd.Next(0, 25); 

            return num;
        }

        private static bool CheckStringExists(string stringToCheck)
        {
            if (used_names.Contains(stringToCheck))
                return true;

            return false;
        }
        public static FieldDefUser CreateField(FieldSig sig)
        {
            return new FieldDefUser(GenerateString(), sig, FieldAttributes.Public | FieldAttributes.Static);
        }
        public static string GenerateString()
        {
            int seed = rnd.Next();
            return (seed * 0x19660D + 0x3C6EF35).ToString("X");
        }

        public static Random rnd = new Random();
    }
}