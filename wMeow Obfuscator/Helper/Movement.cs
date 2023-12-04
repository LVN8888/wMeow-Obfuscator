using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wMeow_Obfuscator.Helper
{
    public class Movement
    {
        public static void ShakeMe(Form form)
        {
            Point location = form.Location;
            Random random = new Random(1337);
            for (int i = 0; i < 10; i++)
            {
                form.Location = new Point(location.X + random.Next(-10, 10), location.Y + random.Next(-10, 10));
                Thread.Sleep(20);
            }
            form.Location = location;
        }
    }
}
