﻿using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class Stloc : Base
	{
		public override void emu()
		{
			var val = All.val.valueStack.Pop();
			var index = All.binr.ReadInt32();
			All.val.locals[index] = val;
		}
	}
}
