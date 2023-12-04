using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Runtime
{
	internal static class AntiDumpRT
	{
		[DllImport("kernel32.dll")]
		private unsafe static extern bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);
		private unsafe static void ADum()
		{
			Module module = typeof(AntiDumpRT).Module;
			byte* ptr = (byte*)((void*)Marshal.GetHINSTANCE(module));
			byte* ptr2 = ptr + 60;
			ptr2 = ptr + *(uint*)ptr2;
			ptr2 += 6;
			ushort num = *(ushort*)ptr2;
			ptr2 += 14;
			ushort num2 = *(ushort*)ptr2;
			ptr2 = ptr2 + 4 + num2;
			byte* ptr3 = stackalloc byte[11];
			uint num3;
			VirtualProtect(ptr2 - 16, 8, 64U, out num3);
			*(int*)(ptr2 - 12) = 0;
			byte* ptr4 = ptr + *(uint*)(ptr2 - 16);
			*(int*)(ptr2 - 16) = 0;
			VirtualProtect(ptr4, 72, 64U, out num3);
			byte* ptr5 = ptr + *(uint*)(ptr4 + 8);
			*(int*)ptr4 = 0;
			*(int*)(ptr4 + 4) = 0;
			*(int*)(ptr4 + (uint)2 * 4) = 0;
			*(int*)(ptr4 + (uint)3 * 4) = 0;
			VirtualProtect(ptr5, 4, 64U, out num3);
			*(int*)ptr5 = 0;
			for (int i = 0; i < (int)num; i++)
			{
				VirtualProtect(ptr2, 8, 64U, out num3);
				Marshal.Copy(new byte[8], 0, (IntPtr)((void*)ptr2), 8);
				ptr2 += 40;
			}
		}
	}
}
