using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AspNetCore.Client.Generator.Data
{
	public class ParameterDefinition
	{
		public MethodDefinition ParentMethod { get; }
		public string Name { get; }
		public string Type { get; }
		public string Default { get; }
		public bool IsRouteVariable { get; }

		public ParameterAttributeOptions Options { get; }


		public ParameterDefinition(
			MethodDefinition method,
			ParameterSyntax parameter)
		{
			ParentMethod = method;



			Name = parameter.Identifier.ValueText.Trim();
			Type = parameter.Type.ToFullString().Trim();
			Default = parameter.Default?.Value.ToFullString().Trim();
			IsRouteVariable = Helpers.IsRouteParameter(Name, ParentMethod.FullRouteTemplate) || (Helpers.IsEnumerable(Type) && Helpers.IsRoutableType(Helpers.GetEnumerableType(Type)));



			var attributes = parameter.AttributeLists.SelectMany(x => x.Attributes).ToList();



			Options = new ParameterAttributeOptions
			{
				Bind = attributes.Any(x => x.Name.ToFullString().StartsWith("Bind")),
				Body = attributes.Any(x => x.Name.ToFullString().StartsWith("FromBody")) || !(Helpers.IsRoutableType(Helpers.GetEnumerableType(Type)))
			};

			var fromQueryAttribute = attributes.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(Constants.FromQuery));
			if (fromQueryAttribute != null)//Fetch route from RouteAttribute
			{
				Options.Query = true;
				Options.QueryName = fromQueryAttribute.ArgumentList?.Arguments.ToFullString().Replace("\"", "").Split('=')[1].Trim();

				if (string.IsNullOrEmpty(Options.QueryName))
				{
					Options.QueryName = Name;
				}
			}

			if ((Helpers.IsRoutableType(Helpers.GetEnumerableType(Type))))
			{
				Options.Query = true;
			}

			if (Options.Query)
			{
				IsRouteVariable = true;
			}

			if (Options.Body)
			{
				IsRouteVariable = false;
			}

		}



		/// <summary>
		/// Name of the parameter inside the route when replacement happens
		/// </summary>
		public string RouteName
		{
			get
			{
				return Name;
				//if (Options.Bind)
				//{
				//	return attribute.Prefix;
				//}
				//else
				//{
				//	return ParameterName;
				//}
			}
		}

		/// <summary>
		/// What is inside the route = "api/{name}", possibly a query string
		/// </summary>
		public string RouteOutput
		{
			get
			{
				//Is route arg a enumerable/array?
				if (Helpers.IsEnumerable(Type))
				{
					string name = null;
					if(Options.QueryName != null)
					{
						name = Options.QueryName;
					}
					else
					{
						name = $"{{nameof({RouteName})}}";
					}

					return $@"{{string.Join(""&"",{RouteName}.Select(x => $""{name}={{{Helpers.GetRouteStringTransform("x", Type)}}}""))}}";
				}
				else if (Default != null || !IsRouteVariable || Options.Query)
				{
					return $"{RouteName}={{{Helpers.GetRouteStringTransform(RouteName, Type)}}}";
				}
				else
				{
					return $"{{{Helpers.GetRouteStringTransform(RouteName, Type)}}}";
				}
			}
		}


		/// <summary>
		/// What goes into the void Get(string name)
		/// </summary>
		public string MethodParameterOutput
		{
			get
			{
				if (Default != null)
				{
					return $"{Type} {RouteName} = {Default}";
				}
				else
				{
					return $"{Type} {RouteName}";
				}
			}
		}

		/// <summary>
		/// What goes into the client.Get(route)/client.Post(route,obj)
		/// </summary>
		public string HttpCallOutput
		{
			get
			{
				if (this.Options.Body)
				{
					return RouteName;
				}
				else
				{
					return null;
				}
			}
		}


		public override string ToString()
		{
			if (Default != null)
			{
				return $"{Type} {Name} = {Default}";
			}
			else
			{
				return $"{Type} {Name}";
			}
		}

	}


	public class ParameterAttributeOptions
	{
		public bool Bind { get; set; }
		public bool Body { get; set; }
		public bool Query { get; set; }
		public string QueryName { get; set; }
	}
}
