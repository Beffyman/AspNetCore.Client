using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.Navigation
{
	public interface INavNode
	{
		IEnumerable<INavNode> GetChildren();
	}
}
