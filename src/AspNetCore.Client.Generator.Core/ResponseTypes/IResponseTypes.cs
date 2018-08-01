﻿using AspNetCore.Client.Generator.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.ResponseTypes
{
	/// <summary>
	/// Detailing that this component contains response types that can be added
	/// </summary>
	public interface IResponseTypes
	{

		/// <summary>
		/// List of response types that can be added to the context
		/// </summary>
		IList<ResponseType> ResponseTypes { get; set; }

	}
}
