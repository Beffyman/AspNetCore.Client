using AspNetCore.Client.Http;
using AspNetCore.Client.Serializers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class HttpSerializerDependency : IDependency
	{
		public string GetDependencyFieldType(string clientName)
		{
			return nameof(IHttpSerializer);
		}

		public string GetDependencyParameterType(string clientName)
		{
			return $"Func<{clientName},{nameof(IHttpSerializer)}>";
		}

		public string GetDependencyName(string clientName)
		{
			return $"Serializer";
		}

		public bool HasAssignmentOverride => true;

		public string GetAssignmentOverride(string value)
		{
			return $"{value}(this)";
		}
	}
}
