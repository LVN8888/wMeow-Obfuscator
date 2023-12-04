using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoxDLibHelper.Resource
{
    internal class RwMeowS
    {
        public static string ByteResource(string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetCallingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return Encoding.UTF8.GetString(ba);
            }
        }
    }
}
