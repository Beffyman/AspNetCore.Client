using AspNetCore.Client.Http;
using AspNetCore.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class HttpSerializerDependency : IDependency
	{
		public string Type => nameof(IHttpSerializer);
		public string Name => "Serializer";
	}
}
