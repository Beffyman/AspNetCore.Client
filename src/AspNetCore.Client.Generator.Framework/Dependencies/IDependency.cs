using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Framework.Dependencies
{
	public interface IDependency
	{
		string GetDependencyFieldType(string clientName);
		string GetDependencyParameterType(string clientName);
		string GetDependencyName(string clientName);
		bool HasAssignmentOverride { get; }
		string GetAssignmentOverride(string value);
	}
}
