using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class Ldnull : Base
	{
		public override void emu()
		{
			All.val.valueStack.Push(null);
		}
	}
}
