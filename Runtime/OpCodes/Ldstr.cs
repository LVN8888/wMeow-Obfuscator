﻿using System;
using System.Collections.Generic;

using System.Text;

using IL_Emulator_Dynamic;

namespace Runtime.OpCodes
{
	class Ldstr : Base
	{
		public override void emu()
		{
			string str = All.binr.ReadString();
			All.val.valueStack.Push(str);
		}
	}
}
