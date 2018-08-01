using System;
using System.Collections.Generic;
using System.Text;
using AspNetCore.Client.Generator.Core.Navigation;

namespace AspNetCore.Client.Generator.Core.RequestModifiers
{
	public class RequestModifier : IRequestModifier
	{
		public RequestModifier()
		{

		}

		public IRequestModifier ExtractModifier()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
