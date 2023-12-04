using System;
using System.Threading;
using System.Windows.Forms;

namespace wMeow_Obfuscator
{
    public partial class fDone : Form
    {
        public fDone()
        {
            InitializeComponent();
        }
        private void siticoneRoundedButton1_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void done_Load(object sender, EventArgs e)
        {
            Opacity = 0.9;
            try
            {
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(2000);
                    try
                    {
                        Close();
                    }
                    catch 
                    {
                    }              
                });
                thread.Start();
                thread.IsBackground = true;
            }
            catch
            {
            }
        }
    }
}
