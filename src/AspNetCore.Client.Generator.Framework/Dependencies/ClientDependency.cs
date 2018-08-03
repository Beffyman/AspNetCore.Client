using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public class ClientDependency : IDependency
	{
		public string Type { get; }

		public string Name => "Client";


		public ClientDependency(string type)
		{
			Type = type;
		}
	}
}
