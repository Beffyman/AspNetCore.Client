using AspNetCore.Client.Http;
using AspNetCore.Client.RequestModifiers;
using AspNetCore.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class RequestModifierDependency : IDependency
	{
		public string Type => nameof(IRequestModifier);
		public string Name => "Modifier";
	}
}
