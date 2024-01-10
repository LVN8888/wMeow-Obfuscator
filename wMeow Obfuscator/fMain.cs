using System;
using System.Drawing;
using System.Windows.Forms;
using RenamingObfuscation;
using Protections;
using System.IO;
using Core;
using System.Data.SqlTypes;
using dnlib.DotNet;
using MeoxDLibHelper;
using System.Collections.Generic;
using MeoxDLibHelper.Renamer;
using ControlFlow = Protections.ControlFlow;
using wMeow_Obfuscator.Helper;
using MLib = MeoxDLibHelper.MLib;
using System.Runtime.InteropServices;
using wMeow_Obfuscator.Protections.Software;

namespace wMeow_Obfuscator
{
    public partial class fMain : Form
    {
        public static string Final;
        public fMain()
        {
            InitializeComponent();
            
        }
        private void Obfuscator_Load(object sender, EventArgs e)
        {         
            Opacity = 0.9;
        }
        #region MForm
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void mPanel_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0xA1, 0x2, 0);
        }
        #endregion
        #region ControlBox
        private void e_btn_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void m_btn_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        #endregion
        #region Ui
        private void HomeButton_Click(object sender, EventArgs e)
        {
            HomeButton.FillColor = Color.FromArgb(18, 18, 18);
            ProtectionsButton.FillColor = Color.FromArgb(23, 23, 23);
            VirtualizerButton.FillColor = Color.FromArgb(23, 23, 23);
            CreditsButton.FillColor = Color.FromArgb(23, 23, 23);
            tabControl1.SelectTab(page_home);
        }
        private void ProtectionsButton_Click(object sender, EventArgs e)
        {
            HomeButton.FillColor = Color.FromArgb(23, 23, 23);
            ProtectionsButton.FillColor = Color.FromArgb(18, 18, 18);
            VirtualizerButton.FillColor = Color.FromArgb(23, 23, 23);
            CreditsButton.FillColor = Color.FromArgb(23, 23, 23);
            tabControl1.SelectTab(page_protections);
        }
        private void VirtualizerButton_Click(object sender, EventArgs e)
        {
            HomeButton.FillColor = Color.FromArgb(23, 23, 23);
            VirtualizerButton.FillColor = Color.FromArgb(18, 18, 18);
            ProtectionsButton.FillColor = Color.FromArgb(23, 23, 23);
            CreditsButton.FillColor = Color.FromArgb(23, 23, 23);
            tabControl1.SelectTab(page_virt);
        }
        private void CreditsButton_Click(object sender, EventArgs e)
        {
            HomeButton.FillColor = Color.FromArgb(23, 23, 23);
            CreditsButton.FillColor = Color.FromArgb(18, 18, 18);
            ProtectionsButton.FillColor = Color.FromArgb(23, 23, 23);
            VirtualizerButton.FillColor = Color.FromArgb(23, 23, 23);
            tabControl1.SelectTab(page_credits);
        }
        #endregion
        #region AddAssembly
        private void add_assembly_Click(object sender, EventArgs e)
        {
            OpenFileDialog x = new OpenFileDialog();
            x.Title = "Load Assembly";
            x.Filter = ".NET Assembly (*.exe)|*.exe|(*.dll)|*.dll";
            x.Multiselect = false;
            if (x.ShowDialog() == DialogResult.OK)
            {
                Assembly_text.Text = x.FileName;
                string text = Assembly_text.Text;
                int num = text.LastIndexOf(".");
                int num2 = text.LastIndexOf("\\");
                if (num != -1)
                {
                    string text2 = text.Substring(num);
                    text2 = text2.ToLower();
                    if (text2 == ".exe" || text2 == ".dll")
                    {
                        Assembly_text.Text = text;
                        string name = text.Substring(num2);
                    }
                }
                checkedListBox1.Items.Clear();
                tempMethodsList.Clear();
                ModuleDefMD moduleDef = ModuleDefMD.Load(Assembly_text.Text);
                foreach (TypeDef type in moduleDef.GetTypes())
                {
                    foreach (MethodDef method in type.Methods)
                    {
                        checkedListBox1.Items.Add(method.FullName);
                        tempMethodsList.Add(method.FullName);
                    }
                }
            }
        }
        private void Assembly_text_DragDrop(object sender, DragEventArgs e)
        {
            Array array = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (array != null)
            {
                string text = array.GetValue(0).ToString();
                int num = text.LastIndexOf(".");
                int num2 = text.LastIndexOf("\\");
                if (num != -1)
                {
                    string text2 = text.Substring(num);
                    text2 = text2.ToLower();
                    if (text2 == ".exe" || text2 == ".dll")
                    {
                        Assembly_text.Text = text;
                        string name = text.Substring(num2);
                    }
                }
                checkedListBox1.Items.Clear();
                tempMethodsList.Clear();
                ModuleDefMD moduleDef = ModuleDefMD.Load(Assembly_text.Text);
                foreach (TypeDef type in moduleDef.GetTypes())
                {
                    foreach (MethodDef method in type.Methods)
                    {
                        checkedListBox1.Items.Add(method.FullName);
                        tempMethodsList.Add(method.FullName);
                    }
                }
            }
        }
        private void Assembly_text_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        #endregion
        #region Obfuscate
        private void obf_assembly_Click(object sender, EventArgs e)
        {
            new MeoxDLibHelper.MLib.MeoLibrary(Assembly_text.Text);
            if (aildasm_checkbox.Checked)
            {
                AntiILDasm.ildasm(MLib.MeoLibrary.moduleDef);
            }
            if (adebug_checkbox.Checked)
            {
                if (Assembly_text.Text.EndsWith("exe"))
                {
                    AntiDebug.Inject(MLib.MeoLibrary.moduleDef);
                }
                
            }
            if (senc_checkbox.Checked)
            {
                ExecuteString();
            }                      
            if (hidemethod_checkbox.Checked)
            {
                if (!adebug_checkbox.Checked)
                {
                    HideMethod.Execute(MLib.MeoLibrary.moduleDef);
                }             
            }
            
            if (cflow_checkbox.Checked)
            {
                ControlFlow.Run(MLib.MeoLibrary.moduleDef);
            }
            if (intenc_checkbox.Checked)
            {
                StringEncryptionBase64.Run(MLib.MeoLibrary.moduleDef);
                IntControlFlow.Run(MLib.MeoLibrary.moduleDef); 
            }
            if (mutation_checkbox.Checked)
            {
                //MeoxDLibHelper.MutationProtection.Execute2(MLib.MeoLibrary.moduleDef);
                MeoxDLibHelper.Mutations.Execute(MLib.MeoLibrary.moduleDef);
            }
            if (junk_checkbox.Checked)
            {
                RandomOutlinedMethods.Add(MLib.MeoLibrary.moduleDef);
                AddJunk.Add(MLib.MeoLibrary.moduleDef);
            }
            if (arith_checkbox.Checked)
            {
                MeoxDLibHelper.Arithmetic.Execute(MLib.MeoLibrary.moduleDef);
            }
            if (calli_checkbox.Checked)
            {
                CallConvertion.Execute();
            }
            if (renaming_checkbox.Checked)
            {
                new OtherRenamer().Execute(MLib.MeoLibrary.moduleDef);
            }
            if (proxystring_checkbox.Checked)
            {
                ProxyString.Execute(MLib.MeoLibrary.moduleDef);
                //NewControlFlow.Execute(MLib.MeoLibrary.moduleDef);
            }
            if (proxyint_checkbox.Checked)
            {
                ProxyInt.Execute(MLib.MeoLibrary.moduleDef);
            }
            if (adump_checkbox.Checked)
            {
                AntiDump.Inject(MLib.MeoLibrary.moduleDef);
            }
            if (http_checkbox.Checked)
            {               
                AntiHttp.Inject(MLib.MeoLibrary.moduleDef);
            }
            if (crack_checkbox.Checked)
            {
                AntiCrack.Inject(MLib.MeoLibrary.moduleDef);
            }
            zAttributes.Add(MLib.MeoLibrary.moduleDef);
            zAttributes.Watermark(MLib.MeoLibrary.moduleDef);
            MLib.Meo.Save();
            Final = Assembly_text.Text;
            Virt();
            MLib.MeoClean.Clean();
            Form d = new fDone();
            d.ShowDialog();
            
        }
        #endregion
        #region Virtualize
        public static string z;
        public void Virt()
        {
            z = null;
            string x = Path.GetFullPath(Final);           
            if (x.EndsWith("exe"))
            {
                string y = x.Substring(0, x.Length - 4);
                z = y + "_Protected.exe";
            }
            if (x.EndsWith("dll"))
            {
                string y = x.Substring(0, x.Length - 4);
                z = y + "_Protected.dll";
            }       
            if (vflag == true)
            {
                try
                {
                    var stex = File.ReadAllBytes(z);
                    List<string> methodList = new List<string>();
                    foreach (string item in checkedListBox1.CheckedItems)
                    {
                        methodList.Add(item);
                    }
                    Protector.Settings(cbbvirstring.Checked, ccbmathvir.Checked, cbbnumobf.Checked, cbbmutationvir.Checked, swanalysis.Value);
                    byte[] assemblyProtected = Protector.Protect(stex, methodList);
                    string newfile = z.Replace(".exe","") + "-VM.exe";
                    if (z.EndsWith(".exe"))
                    {
                         newfile = z.Replace(".exe", "") + "-VM.exe";
                    }
                    if (z.EndsWith(".dll"))
                    {
                         newfile = z.Replace(".dll", "") + "-VM.dll";
                    }
                    File.WriteAllBytes(newfile, assemblyProtected);
                    assemblyProtected = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"Log");
                }
                z = null;
                
                Protector.moduleDefMD = null;
            }           
        }
        #endregion
        private void siticoneImageButton1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/siuiuNana");
        }
        public static bool vflag;
        private void virt_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (virt_checkbox.Checked == true)
            {
                vflag = true;
            }
            if (virt_checkbox.Checked == false)
            {
                vflag = false;
            }
        }

        private void SwString_ValueChanged(object sender, EventArgs e)
        {
            switch (SwString.Value)
            {
                case 0:
                    StringStatus.Text = "Off";
                    break;
                case 1:
                    StringStatus.Text = "Aggressive";
                    return;
                case 2:
                    StringStatus.Text = "Normal";
                    return;
                case 3:
                    StringStatus.Text = "Strong";
                    return;
                case 4:
                    StringStatus.Text = "Maximum";
                    return;
            }
        }
        public void ExecuteString()
        {
            switch (SwString.Value)
            {
                case 0:
                    break; 
                case 1:
                    //StringEncryptSplit.Execute(MLib.MeoLibrary.moduleDef);
                    StringEncoder.Execute(MLib.MeoLibrary.moduleDef);
                    break;
                case 2:
                    new EConstants().Inject();
                    MeoxDLibHelper.RemoveObfuscator.Execute(MLib.MeoLibrary.moduleDef);
                    //StringToArray.Execute(MLib.MeoLibrary.moduleDef);
                    break;
                case 3:
                    StringEncryptionBase64.Run(MLib.MeoLibrary.moduleDef);
                    StringEncryptionPrivate.Execute(MLib.MeoLibrary.moduleDef);
                    break;
                case 4:
                    StringEncryptionBase64.Run(MLib.MeoLibrary.moduleDef);
                    MeoxDLibHelper.StringEncryptionASCII.Execute(MLib.MeoLibrary.moduleDef);
                    //MeoxDLibHelper.Resource.ProxyResource.Execute(MLib.MeoLibrary.moduleDef);
                    /*StringEncoder.Execute(MLib.MeoLibrary.moduleDef);
                    StringEncryptionBase64.Run(MLib.MeoLibrary.moduleDef);
                    StringEncryptionPrivate.Execute(MLib.MeoLibrary.moduleDef);*/
                    break;
            }
        }

        private void senc_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SwString.Enabled = senc_checkbox.Checked;
        }

        private void SwRecommen_CheckedChanged(object sender, EventArgs e)
        {
            if (SwRecommen.Checked)
            {
                Invoke((MethodInvoker)delegate
                {
                    adump_checkbox.Checked = true;
                    adebug_checkbox.Checked = true;
                    virt_checkbox.Checked = true;
                    //senc_checkbox.Checked = true;
                    //cbbvirstring.Checked = true;
                    //SwString.Value = 1;
                    cbbstringanalysis.Checked = true;
                    swanalysis.Value = 2;
                    cbbvirstring.Checked = true;
                });
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    adump_checkbox.Checked = false;
                    adebug_checkbox.Checked = false;
                    virt_checkbox.Checked = false;
                    //senc_checkbox.Checked = false;
                    //cbbvirstring.Checked = false;
                    //SwString.Value = 0;
                    cbbstringanalysis.Checked = false;
                    swanalysis.Value = 0;
                    cbbvirstring.Checked = false;
                });
            }
        
       }

        private void Obfuscator_Shown(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void swanalysis_ValueChanged(object sender, EventArgs e)
        {
            Dictionary<int, string> analysisStatus = new Dictionary<int, string>()
            {
               { 0, "Off" },
               { 1, "Replace" },
               { 2, "Remove" },
               { 3, "Strong" }
            };

            if (analysisStatus.TryGetValue(swanalysis.Value, out string status))
            {
                StringAnalysisStatus.Text = status;
            }

        }

        private void cbbstringanalysis_CheckedChanged(object sender, EventArgs e)
        {
            swanalysis.Enabled = cbbstringanalysis.Checked;
            if (!cbbstringanalysis.Checked)
            {
                swanalysis.Value = 0;
            }
        }

        private void Animation_Tick(object sender, EventArgs e)
        {
            Movement.ShakeMe(this);
            this.Animation.Enabled = false;
        }

        private void VirtualizerListButton_Click(object sender, EventArgs e)
        {
            HomeButton.FillColor = Color.FromArgb(23, 23, 23);
            ProtectionsButton.FillColor = Color.FromArgb(23, 23, 23);
            VirtualizerButton.FillColor = Color.FromArgb(23, 23, 23);
            CreditsButton.FillColor = Color.FromArgb(23, 23, 23);
            VirtualizerListButton.FillColor = Color.FromArgb(18, 18, 18);
            tabControl1.SelectTab(page_virtList);
        }
        readonly List<string> tempMethodsList = new List<string>();
        private void siticoneTextBox2_TextChanged(object sender, EventArgs e)
        {
            string text = this.siticoneTextBox2.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                CheckedListBox.CheckedItemCollection checkedItems = checkedListBox1.CheckedItems;
                for (int i = checkedListBox1.Items.Count - 1; i >= 0; i--)
                {
                    bool flag = false;
                    foreach (object obj in checkedItems)
                    {
                        if (checkedListBox1.Items[i].Equals(obj))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        checkedListBox1.Items.RemoveAt(i);
                    }
                }
                using (List<string>.Enumerator enumerator2 = tempMethodsList.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        string text2 = enumerator2.Current;
                        if (text2.ToUpper().Contains(text.ToUpper()) && !checkedListBox1.Items.Contains(text2))
                        {
                            checkedListBox1.Items.Add(text2);
                        }
                    }
                    return;
                }
            }
            foreach (string text3 in this.tempMethodsList)
            {
                if (!checkedListBox1.Items.Contains(text3))
                {
                    checkedListBox1.Items.Add(text3);
                }
            }
        }

        private void cbSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSelectAll.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }
    }
}
