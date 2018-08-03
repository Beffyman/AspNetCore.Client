using AspNetCore.Client.Generator.Framework;
using AspNetCore.Client.Generator.Framework.AttributeInterfaces;
using AspNetCore.Client.Generator.Framework.Dependencies;
using AspNetCore.Client.Generator.Framework.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace AspNetCore.Client.Generator.Temp
{
	public static class ClassWritier
	{

		public static string WriteController(Controller controller)
		{
			return
$@"
{(controller.NamespaceSuffix != null ? $@"namespace {controller.NamespaceSuffix}
{{" : string.Empty)}

{WriteClassInterface(controller)}

{WriteClassImplementation(controller)}

{(controller.NamespaceSuffix != null ? $@"}}" : string.Empty)}
";
		}

		#region Class

		private static string WriteClassInterface(Controller controller)
		{
			return
$@"
{GetObsolete(controller)}
	public interface I{controller.Name} : I{Settings.ClientInterfaceName}
	{{
{string.Join($"{Environment.NewLine}", controller.Endpoints.Select(WriteEndpointInterface))}
	}}
";
		}

		private static string WriteClassImplementation(Controller controller)
		{
			var dependencies = controller.GetInjectionDependencies().ToList();
			dependencies.Insert(0, new ClientDependency($"I{Settings.ClientInterfaceName}Wrapper"));

			return
$@"
{GetObsolete(controller)}
	{(Settings.UseInternalClients ? "internal" : "public")} class {controller.Name} : I{controller.Name}
	{{
{string.Join($"{Environment.NewLine}", dependencies.Select(WriteDependenciesField))}

		public {controller.Name}({string.Join($",{Environment.NewLine}", dependencies.Select(WriteDependenciesParameter))})
		{{
{string.Join($"{Environment.NewLine}", dependencies.Select(WriteDependenciesAssignment))}
		}}

{string.Join($"{Environment.NewLine}", controller.Endpoints.Select(WriteEndpointImplementation))}
	}}
";
		}

		#endregion Class

		#region Dependencies

		public static string WriteDependenciesField(IDependency dependency)
		{
			return $@"		protected readonly {dependency.Type} {dependency.Name};";
		}

		public static string WriteDependenciesParameter(IDependency dependency)
		{
			return $@"{dependency.Type} param_{dependency.Name.ToLower()}";
		}

		public static string WriteDependenciesAssignment(IDependency dependency)
		{
			return $@"{dependency.Name} = param_{dependency.Name.ToLower()};";
		}

		#endregion Dependencies


		#region Endpoint

		private static string WriteEndpointInterface(Endpoint endpoint)
		{
			return
$@"
{GetObsolete(endpoint)}
{GetInterfaceReturnType(endpoint.ReturnType, false)} {endpoint.Name}
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
);

{GetObsolete(endpoint)}
{GetInterfaceReturnType(nameof(HttpResponseMessage), false)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
);

{GetObsolete(endpoint)}
{GetInterfaceReturnType(endpoint.ReturnType, true)} {endpoint.Name}Async
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
);

{GetObsolete(endpoint)}
{GetInterfaceReturnType(nameof(HttpResponseMessage), true)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
);

";
		}

		private static string WriteEndpointImplementation(Endpoint endpoint)
		{
			return
$@"

{GetObsolete(endpoint)}
public {GetImplementationReturnType(endpoint.ReturnType, false)} {endpoint.Name}
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
)
{{
{GetResponseMethod(endpoint,false)}
}}

{GetObsolete(endpoint)}
public {GetImplementationReturnType(nameof(HttpResponseMessage), false)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
)
{{
{GetRawMethod(endpoint, false)}
}}

{GetObsolete(endpoint)}
public {GetImplementationReturnType(endpoint.ReturnType, true)} {endpoint.Name}Async
(
{string.Join($",{Environment.NewLine}", endpoint.GetParameters().Select(GetParameter))}
)
{{
{GetResponseMethod(endpoint, true)}
}}

{GetObsolete(endpoint)}
public {GetImplementationReturnType(nameof(HttpResponseMessage), true)} {endpoint.Name}Raw
(
{string.Join($",{Environment.NewLine}", endpoint.GetParametersWithoutResponseTypes().Select(GetParameter))}
)
{{
{GetRawMethod(endpoint, true)}
}}

";
		}

		#endregion Endpoint

		private static string GetResponseMethod(Endpoint endpoint, bool async)
		{

		}

		private static string GetRawMethod(Endpoint endpoint, bool async)
		{

		}

		private static string GetInterfaceReturnType(string returnType, bool async)
		{
			if (async)
			{
				if (returnType == null)
				{
					return $"Task";
				}
				else
				{
					return $"{Helpers.GetTaskType()}<{returnType}>";
				}
			}
			else
			{
				if (returnType == null)
				{
					return $"void";
				}
				else
				{
					return $"{returnType}";
				}
			}
		}

		private static string GetImplementationReturnType(string returnType, bool async)
		{
			if (async)
			{
				if (returnType == null)
				{
					return $"async Task";
				}
				else
				{
					return $"async {Helpers.GetTaskType()}<{returnType}>";
				}
			}
			else
			{
				if (returnType == null)
				{
					return $"void";
				}
				else
				{
					return $"{returnType}";
				}
			}
		}

		private static string GetParameter(IParameter parameter)
		{
			return $@"{parameter.Type} {parameter.Name}{(parameter.DefaultValue != null ? $" = {parameter.DefaultValue}" : $"")}";
		}

		private static string GetObsolete(IObsolete ob)
		{
			if (ob.Obsolete)
			{
				return $@"	[{Constants.Obsolete}(""{ob.ObsoleteMessage}"")]";
			}
			else
			{
				return string.Empty;
			}

		}
	}
}
