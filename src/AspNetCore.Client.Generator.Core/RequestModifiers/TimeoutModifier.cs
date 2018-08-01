using AspNetCore.Client.Generator.Core.Navigation;
using AspNetCore.Client.Generator.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.RequestModifiers
{
	public class TimeoutModifier : IRequestModifier, IParameter
	{

		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name => "timeout";

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type => $"{nameof(TimeSpan)}?";

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue => "null";

		public TimeoutModifier()
		{

		}


		public IRequestModifier ExtractModifier()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}
	}
}
