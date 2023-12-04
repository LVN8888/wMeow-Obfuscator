using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class Pop : Base
	{
		public override void emu()
		{
			dynamic var =All.val.valueStack.Pop();
		}
	}
}
