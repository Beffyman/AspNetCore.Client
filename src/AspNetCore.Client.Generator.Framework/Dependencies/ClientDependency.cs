using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class ClientDependency : IDependency
	{
		private string _type { get; }

		public string GetDependencyFieldType(string clientName)
		{
			return GetDependencyParameterType(clientName);
		}

		public string GetDependencyParameterType(string clientName)
		{
			return _type;
		}

		public string GetDependencyName(string clientName)
		{
			return $"Client";
		}

		public bool HasAssignmentOverride => false;

		public string GetAssignmentOverride(string value)
		{
			throw new NotImplementedException();
		}


		public ClientDependency(string type)
		{
			_type = type;
		}
	}
}
