using AspNetCore.Client.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class HttpOverrideDependency : IDependency
	{
		public string GetDependencyFieldType(string clientName)
		{
			return nameof(IHttpOverride);
		}

		public string GetDependencyParameterType(string clientName)
		{
			return $"Func<{clientName},{nameof(IHttpOverride)}>";
		}

		public string GetDependencyName(string clientName)
		{
			return $"HttpOverride";
		}

		public bool HasAssignmentOverride => true;

		public string GetAssignmentOverride(string value)
		{
			return $"{value}(this)";
		}
	}
}
