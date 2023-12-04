using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class ConvU1 : Base
	{
		public override void emu()
		{
			var val = All.val.valueStack.Pop();
			byte bt = Convert.ToByte( val);
			All.val.valueStack.Push((int)bt);
		}
	}
}
