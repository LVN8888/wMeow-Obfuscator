using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Runtime
{
	internal static class AntiDebugRT
	{
		[DllImport("kernel32.dll")]
		internal static extern int CloseHandle(IntPtr hModule);
		[DllImport("kernel32.dll")]
		internal static extern IntPtr OpenProcess(uint hModule, int procName, uint procId);
		[DllImport("kernel32.dll")]
		internal static extern uint GetCurrentProcessId();
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string hModule);
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		internal static extern GetProcA GetProcAddress(IntPtr hModule, string procName);
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress")]
		internal static extern GetProcA2 GetProcAddress_2(IntPtr hModule, string procName);

		internal static void Detected()
		{
			try
			{
				IntPtr hModule = LoadLibrary("kernel32.dll");
				if (Debugger.IsAttached)
				{
                    Environment.Exit(0);
                }
				GetProcA procAddress = GetProcAddress(hModule, "IsDebuggerPresent");
				if (procAddress != null && procAddress() != 0)
				{
                    Environment.Exit(0);
                }
				IntPtr intPtr = OpenProcess(1024U, 0, GetCurrentProcessId());
				if (intPtr != IntPtr.Zero)
				{
					try
					{
						GetProcA2 procAddress_ = GetProcAddress_2(hModule, "CheckRemoteDebuggerPresent");
						if (procAddress_ != null)
						{
							int num = 0;
							if (procAddress_(intPtr, ref num) != 0)
							{
								if (num != 0)
								{
                                    Environment.Exit(0);
                                }
							}
						}
					}
					finally
					{
						CloseHandle(intPtr);
					}
				}
				try
				{
					CloseHandle(new IntPtr(305419896));
				}
				catch
				{
                    Environment.Exit(0);
                }
			}
			catch
			{
			}
		}
		internal delegate int GetProcA();
		internal delegate int GetProcA2(IntPtr hProcess, ref int pbDebuggerPresent);
		internal delegate int WL(IntPtr wnd, IntPtr lParam);
		internal delegate int GetProcA3(WL lpEnumFunc, IntPtr lParam);
	}
}
