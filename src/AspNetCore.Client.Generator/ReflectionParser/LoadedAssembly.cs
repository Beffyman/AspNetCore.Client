using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using AspNetCore.Client.Generator.Framework;

namespace AspNetCore.Client.Generator.ReflectionParser
{
	public class LoadedAssembly
	{
		public Assembly File { get; }

		public IEnumerable<Type> ControllerTypes { get; }

		public GenerationContext Context { get; }

		public bool Failed { get; }
		public bool UnexpectedFailure { get; }
		public string Error { get; }

		public LoadedAssembly(string file)
		{
			File = Assembly.LoadFile(file);

			Context = new GenerationContext();

			var baseControllerType = typeof(ControllerBase);

			ControllerTypes = File.GetTypes()
				.Where(x => baseControllerType.IsAssignableFrom(x)
							&& !x.IsAbstract)
				.ToList();

			try
			{
				foreach (var controller in ControllerTypes)
				{

				}
			}
			catch (NotSupportedException nse)
			{
				Failed = true;
				Error = nse.Message;
			}
#if !DEBUG
			catch (Exception ex)
			{
				Failed = true;
				UnexpectedFailure = true;
				Error = ex.Message;
			}
#endif

		}



	}
}
