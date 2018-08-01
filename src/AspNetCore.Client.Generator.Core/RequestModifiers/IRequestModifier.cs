using AspNetCore.Client.Generator.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.RequestModifiers
{
	public interface IRequestModifier : INavNode
	{
		IRequestModifier ExtractModifier();
	}
}
