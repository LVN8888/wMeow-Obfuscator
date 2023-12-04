using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace wMeow_Obfuscator.Protections.Software
{
    internal class AntiHttpRuntime
    {
        public static void Initialize()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.CheckCertificateRevocationList = true;
            new Thread(delegate ()
            {
                Init();
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            }.Start();
        }

        public static void Init()
        {
            while (true)
            {
                try
                {             
                    HttpWebRequest httpWebRequest = WebRequest.Create("https://google.com") as HttpWebRequest;
                    httpWebRequest.Timeout = 10000;
                    httpWebRequest.ContinueTimeout = 10000;
                    httpWebRequest.ReadWriteTimeout = 10000;
                    httpWebRequest.KeepAlive = true;
                    httpWebRequest.ServerCertificateValidationCallback = null;
                    try
                    {
                        using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                        {
                            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                            {
                                httpWebResponse.Close();
                            }
                            else
                            {
                                httpWebResponse.Close();
                                Message("Anti HTTP debugging 1");
                                Environment.Exit(0);
                            }
                        }
                        Thread.Sleep(3000);
                    }
                    catch
                    {
                        Message("Anti HTTP debugging 2");
                        Environment.Exit(0);
                    }
                }
                catch
                {
                    Message("Anti HTTP debugging 3");
                    Environment.Exit(0);
                }
            }
        }

        static void Message(string message = null)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            File.WriteAllText(path + "\\Warning.txt", message);

            Process notepad = new Process();
            notepad.StartInfo.FileName = "notepad.exe";
            notepad.StartInfo.Arguments = path + "\\Warning.txt";
            notepad.Start();
        }
    }
}
