using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace wMeow_Obfuscator.Protections.Software
{
    internal class AntiCrackRuntime
    {
        public static void InitCrack()
        {
            Thread thread = new Thread(() =>
            {
                AntiC();
            });
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            thread.Start();
        }

        private static void AntiC()
        {
            while (true)
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\wMeow");
                try
                {
                    if (registryKey != null && registryKey.GetValue("c").ToString() == "banned")
                    {
                        File.WriteAllText(Directory.GetCurrentDirectory() + "\\Warning.txt", "Banned With AntiCrack !");
                        Process.Start("notepad.exe", Directory.GetCurrentDirectory() + "\\Warning.txt");
                        Environment.Exit(0);
                    }
                }
                catch
                {
                }
                string[] array = new string[]
                {
                    "CosMos",
                    "SimpleAssemblyExplorer",
                    "StringDecryptor",
                    "CodeCracker",
                    "x32dbg",
                    "x64dbg",
                    "ollydbg",
                    "simpleassembly",
                    "httpanalyzer",
                    //"httpdebug",
                    "KsDumper",
                    "fiddler",
                    "processhacker",
                    "solarwinds",
                    //"HTTPDebuggerSvc",
                    "netcheat",
                    "megadumper",
                    "ilspy",
                    "reflector",
                    "exeinfope",
                    "DetectItEasy",
                    "Exeinfo PE",
                    "Process Hacker",
                    //"HTTP Debugger",
                    "dnSpy",
                    "scylla",
                    //"IMMUNITYDEBUGGER",
                    "MegaDumper",
                    "reshacker",
                    "cheat engine",
                    "scylla_x86",
                    "scylla_x64",
                    "Fiddler Everywhere",
                    "ExtremeDumper",
                    "ollydbg",
                    "HxD",
                    "dumper",
                    //"Progress Telerik Fiddler Web Debugger",
                    "dnSpy-x86",
                    "cheat engine",
                    "Cheat Engine",
                    "cheatengine",
                    "cheatengine-x86_64",
                    //"HTTPDebuggerUI",
                    "ProcessHacker",
                    "x32dbg",
                    "x64dbg",
                    /*"DotNetDataCollector32",
                    "DotNetDataCollector64",
                    "CFF Explorer",
                    "M*3*G*4**D*u*m*p*3*R*",
                    "ĘẍtŗęḿęĎựḿҏęŗ",
                    "solarwinds",*/
                    /*"HTTPDebuggerSvc",
                    "HTTPDebuggerUI",*/
                    /*"Everything",
                    "FileActivityWatch",
                    "netcheat"*/
                };
                foreach (Process process in Process.GetProcesses())
                {
                    foreach (string text in array)
                    {
                        if (!process.ProcessName.ToLower().Contains(text.ToLower())) continue;
                        /*string text2 = "easyanticheat";
                        string text3 = "eac";
                        if (process.ProcessName.ToLower().Contains(text2.ToLower()) || process.ProcessName.ToLower().Contains(text3.ToLower())) return;*/
                        try
                        {
                            /*process.Kill();
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            File.WriteAllText(path + "\\Warning.txt", "AntiCrack !");
                            Process.Start("notepad.exe", path + "\\Warning.txt");
                            Environment.Exit(0);*/

                            RegistryKey registryKey2 = Registry.CurrentUser.CreateSubKey("Software\\wMeow");
                            registryKey2.SetValue("c", "banned");
                            registryKey2.Close();
                            process.Kill();
                            //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Warning.txt", "Banned With AntiCrack !");
                            Process.Start("notepad.exe", Directory.GetCurrentDirectory() + "\\Warning.txt");
                            Environment.Exit(0);

                        }
                        catch
                        {
                            process.Close();
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            File.WriteAllText(path + "\\Warning.txt", "AntiCrack !");
                            Process.Start("notepad.exe", path + "\\Warning.txt");
                            Environment.Exit(0);
                        }
                    }
                }
                Thread.Sleep(3000);
            }
        }
    }
}
