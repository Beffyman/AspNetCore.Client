﻿using AspNetCore.Client.Generator.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AspNetCore.Client.Generator.Core.RequestModifiers
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

		public CancellationTokenModifier()
		{

		}



		public IRequestModifier ExtractModifier()
		{
			throw new NotImplementedException();
		}
	}
}
