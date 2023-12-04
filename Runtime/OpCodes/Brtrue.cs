using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class Brtrue : Base
	{
		public override void emu()
		{
			var pos = All.binr.ReadInt32();
			var val = All.val.valueStack.Pop();
			if ((int)val != 0)
				All.binr.BaseStream.Position = pos;
			else
			{
				return;
			}
		}
	}
}
