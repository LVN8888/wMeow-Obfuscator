using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class Ldc : Base
	{
		public override void emu()
		{
			var val = All.binr.ReadInt32();
			All.val.valueStack.Push(val);
		}
	}
}
