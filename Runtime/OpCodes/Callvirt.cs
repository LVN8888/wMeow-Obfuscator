using System;
using System.Collections.Generic;

using System.Reflection;


namespace Runtime.OpCodes
{
	class Callvirt : Base
	{
		public static Dictionary<int,MethodBase> cache = new Dictionary<int, MethodBase>();
		public override void emu()
		{
			All.binr.ReadInt32();
			

		
			var typ = new object[2];
			for (int i = typ.Length; i > 0; i--)
			{
				typ[i-1] = All.val.valueStack.Pop();

			}
				
				
			var type = (Random)All.val.valueStack.Pop();

			All.val.valueStack.Push(type.Next(0, 100)+1);
		}
	}
}
