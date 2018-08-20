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

		public string GetDependencyFieldType(string clientName)
		{
			return GetDependencyParameterType(clientName);
		}

		public string GetDependencyParameterType(string clientName)
		{
			return nameof(IHttpRequestModifier);
		}

		public string GetDependencyName(string clientName)
		{
			return "Modifier";
		}

		public bool HasAssignmentOverride => false;

		public string GetAssignmentOverride(string value)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
