﻿using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class Ldlen : Base
	{
		public override void emu()
		{
			byte[] dy2n =(byte[]) All.val.valueStack.Pop();
			All.val.valueStack.Push(dy2n.Length);
		}
	}
}
