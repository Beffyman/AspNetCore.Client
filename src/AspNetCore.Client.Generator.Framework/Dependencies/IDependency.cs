using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public interface IDependency
	{
		string Type { get; }
		string Name { get; }

	}
}
