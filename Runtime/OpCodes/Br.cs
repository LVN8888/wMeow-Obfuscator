﻿using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class Br : Base
	{
		public override void emu()
		{
			var a = All.binr.ReadInt32();
			All.binr.BaseStream.Position = a;
		}
	}
}
