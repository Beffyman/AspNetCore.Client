﻿using AspNetCore.Client.Generator.Framework.Navigation;
using AspNetCore.Client.Generator.Framework.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.Client.Generator.Framework.RequestModifiers
{
	public class CancellationTokenModifier : IRequestModifier, IParameter
	{

		/// <summary>
		/// Display name of the parameter
		/// </summary>
		public string Name => "cancellationToken";

		/// <summary>
		/// Type of the parameter
		/// </summary>
		public string Type => $"{nameof(CancellationToken)}";

		/// <summary>
		/// What the default value of the parameter is, if it has one. the string "null" should be used for an optional parameter
		/// </summary>
		public string DefaultValue => "default";
		public int SortOrder => 10;


		public IEnumerable<INavNode> GetChildren()
		{
			return null;
		}

		public string ToString()
		{
			return $"{Type} {Name} = {DefaultValue}";
		}
	}
}
