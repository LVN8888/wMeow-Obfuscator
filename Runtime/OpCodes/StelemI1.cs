using System;
using System.Collections.Generic;

using System.Text;


namespace Runtime.OpCodes
{
	class StelemI1 : Base
	{
		public override void emu()
		{
			var value = All.val.valueStack.Pop();
			var index = All.val.valueStack.Pop();
			var array =(byte[]) All.val.valueStack.Pop();
			array[Convert.ToInt32(index)] =Convert.ToByte(Convert.ToInt32( value));
		}
	}
}
