using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.RequestModifiers;
using AspNetCore.Client.Http;
using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class RequestModifierDependency : IDependency, IRequestModifier
	{
		public string Type => nameof(IHttpRequestModifier);
		public string Name => "Modifier";

		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
