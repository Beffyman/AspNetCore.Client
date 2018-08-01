﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Generator.Core.Parameters
{
	public class BodyParameter : IParameter
	{
		public string Name { get; }

		public string Type { get; }

		public string DefaultValue { get; }

		public BodyParameter(string name, string type, string defaultValue = null)
		{
			Name = name;
			Type = type;
			DefaultValue = defaultValue;
		}
	}
}
