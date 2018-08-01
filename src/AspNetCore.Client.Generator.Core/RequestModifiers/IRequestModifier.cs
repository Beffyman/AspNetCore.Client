using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.RequestModifiers
{
	public interface IRequestModifier
	{
		IRequestModifier ExtractModifier();
	}
}
