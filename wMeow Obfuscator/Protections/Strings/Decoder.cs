using System.Reflection;
using System.Text;

namespace Protections
{
    internal static class Decoder
    {
        public static string SERT(string encodedString, int key)
        {
            if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
            {
                var stringBuilder = new StringBuilder();
                foreach (var symbol in encodedString)
                    stringBuilder.Append((char) (symbol ^ key));
                return stringBuilder.ToString();
            }
            return null;
        }
    }
}