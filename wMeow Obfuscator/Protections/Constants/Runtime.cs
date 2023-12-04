using System.Text;

namespace Runtime
{
    internal class CRuntime
    {
        public static string CRT(string text, string key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                uint num = (uint)c;
                int index = i % key.Length;
                char c2 = key[index];
                uint num2 = (uint)c2;
                uint num3 = num ^ num2;
                char value = (char)num3;
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
        }
    }
}
