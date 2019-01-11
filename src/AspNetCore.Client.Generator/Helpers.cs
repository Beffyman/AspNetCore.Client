using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Client.Generator.CSharp.AspNetCoreHttp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace AspNetCore.Client.Generator
{
	internal static class Helpers
	{
		public static K GetValueOrDefault<T, K>(this IDictionary<T, K> source, T key)
		{
			if (source.ContainsKey(key))
			{
				return source[key];
			}
			return default;
		}

		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source) where T : class
		{
			return source.Where(x => x != null);
		}
		public static IEnumerable<T> NotOfType<T, K>(this IEnumerable<T> source) where K : T
		{
			return source.Where(x => !typeof(K).IsAssignableFrom(x.GetType()));
		}

		public static HttpMethod HttpMethodFromEnum(HttpAttributeType type)
		{
			switch (type)
			{
				case HttpAttributeType.Delete:
					return HttpMethod.Delete;
				case HttpAttributeType.Get:
					return HttpMethod.Get;
				case HttpAttributeType.Patch:
					return new HttpMethod("PATCH");
				case HttpAttributeType.Post:
					return HttpMethod.Post;
				case HttpAttributeType.Put:
					return HttpMethod.Put;
				default:
					throw new NotSupportedException($"HttpAttributeType of value {type} is not supported");
			}
		}

		public static T EnumParse<T>(string value) where T : Enum
		{
			if (value == null)
			{
				return default(T);
			}

			return (T)Enum.Parse(typeof(T), value);
		}

		public static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
		{
			for (int numTries = 0; numTries < 25; numTries++)
			{
				FileStream fs = null;
				try
				{
					fs = new FileStream(fullPath, mode, access, share);
					return fs;
				}
				catch (IOException)
				{
					if (fs != null)
					{
						fs.Dispose();
					}
					Thread.Sleep(100);
				}
				catch (UnauthorizedAccessException)
				{
					if (fs != null)
					{
						fs.Dispose();
					}
					Thread.Sleep(100);
				}
			}

			return null;
		}

		public static string SafelyReadFromFile(string path)
		{
			using (var fileStream = WaitForFile(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			using (var reader = new StreamReader(fileStream))
			{
				return reader.ReadToEnd();
			}
		}

		public static void SafelyDeleteFile(string path)
		{
			using (var fileStream = WaitForFile(path, FileMode.Truncate, FileAccess.ReadWrite, FileShare.Delete))
			{
				File.Delete(path);
			}
		}

		public static void SafelyWriteToFile(string path, string text)
		{
			if (!File.Exists(path))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.WriteAllText(path, text);
			}
			else
			{
				using (var fileStream = WaitForFile(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					fileStream.SetLength(0);
					var data = Encoding.UTF8.GetBytes(text);
					fileStream.Write(data, 0, data.Length);
				}
			}
		}



		private static readonly JsonSerializerSettings SETTINGS = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			NullValueHandling = NullValueHandling.Include,
			MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
		};

		public static string SerializeToJson(this object obj)
		{
			return JsonConvert.SerializeObject(obj, SETTINGS);
		}

		public static T DeserializeFromJson<T>(this string str)
		{
			return JsonConvert.DeserializeObject<T>(str, SETTINGS);
		}

		public static object DeserializeFromJson(this string str, Type t)
		{
			return JsonConvert.DeserializeObject(str, t, SETTINGS);
		}



		private static string[] KnownEnumerables = new string[]
		{
		"IEnumerable<(.+)>",
		"IList<(.+)>",
		"ICollection<(.+)>",
		@"(.+)\[\]",
		"List<(.+)>",
		"HashSet<(.+)>",
		"Queue<(.+)>",
		"Stack<(.+)>",
		"SortedSet<(.+)>",
		"LinkedList<(.+)>"
		};

		public static string BackToRouteParameter(this TemplatePart templatePart)
		{
			string route = "{";

			route += templatePart.Name;


			if (templatePart.InlineConstraints?.Any() ?? false)
			{
				foreach (var ic in templatePart.InlineConstraints)
				{
					route += $":{ic.Constraint}";
				}
			}


			if (templatePart.IsOptional)
			{
				route += "?";
			}

			if (templatePart.DefaultValue != null)
			{
				route += $"={templatePart.DefaultValue}";
			}


			return route + "}";
		}

		public static bool IsEnumerable(string type)
		{
			if (type == null)
			{
				return false;
			}

			var listMatch = new Regex($"^(?!, )({string.Join("|", KnownEnumerables)})");
			return listMatch.IsMatch(type);
		}

		public static string GetEnumerableType(string type)
		{
			var listMatch = new Regex($"{string.Join("|", KnownEnumerables)}");
			var match = listMatch.Match(type);
			if (match.Success)
			{
				var groups = (match.Groups as IEnumerable).Cast<Group>();
				var group = groups.Skip(1).Where(x => !string.IsNullOrEmpty(x.Value)).First();
				return group.Value;
			}
			else
			{
				return type;
			}
		}


		public static string[] KnownPrimitives = new string[]
		{
		"char",typeof(char).Name,
		"byte",typeof(byte).Name,
		"sbyte",typeof(sbyte).Name,
		"ushort",typeof(ushort).Name,
		"int",typeof(int).Name,
		"uint",typeof(uint).Name,
		"long",typeof(long).Name,
		"ulong",typeof(ulong).Name,
		"float",typeof(float).Name,
		"double",typeof(double).Name,
		"string",typeof(string).Name,
		"bool",typeof(bool).Name,
		"DateTime",typeof(DateTime).Name,
		"DateTimeOffset",typeof(DateTimeOffset).Name,
		"Guid",typeof(Guid).Name,
		};

		private static Regex NULLABLE_MATCHER = new Regex(@"((.+)\?)|(Nullable<(.+)>)");

		private static string ConvertFromNullable(string type, out bool wasNullable)
		{
			var matches = NULLABLE_MATCHER.Matches(type);
			if (matches.Count > 0)
			{
				type = matches[matches.Count - 1].Groups[2].Value;
				wasNullable = true;
			}
			else
			{
				wasNullable = false;
			}

			return type;
		}

		public static bool IsRoutableType(string type)
		{
			type = ConvertFromNullable(type, out _);

			return KnownPrimitives.Contains(type, StringComparer.CurrentCultureIgnoreCase);
		}

		public static bool IsRouteParameter(string name, string fullRouteTemplate)
		{
			var routeArgs = fullRouteTemplate.GetRouteParameters();
			return routeArgs.ContainsKey(name);
		}

		public static string GetDefaultRouteConstraint(string name, string fullRouteTemplate)
		{
			var routeArgs = fullRouteTemplate.GetRouteParameters();
			if (routeArgs.ContainsKey(name))
			{
				return routeArgs[name].defaultValue;
			}

			return null;
		}

		public static string GetRouteStringTransform(string parameterName, string type)
		{
			var transforms = new Dictionary<string, string>
			{
				{ nameof(DateTime), "{0}.ToString(\"s\", System.Globalization.CultureInfo.InvariantCulture)" },
				{ nameof(DateTimeOffset), "{0}.ToString(\"s\", System.Globalization.CultureInfo.InvariantCulture)" },
				{ $"{nameof(DateTime)}?", "{0}?.ToString(\"s\", System.Globalization.CultureInfo.InvariantCulture)" },
				{ $"{nameof(DateTimeOffset)}?", "{0}?.ToString(\"s\", System.Globalization.CultureInfo.InvariantCulture)" }
			};

			if (IsEnumerable(type))
			{
				type = GetEnumerableType(type);
			}

			type = ConvertFromNullable(type, out bool wasNullable);

			if (wasNullable)
			{
				type = $"{type}?";
			}

			if (transforms.ContainsKey(type))
			{
				return string.Format(transforms[type], parameterName);
			}
			else
			{
				return parameterName;
			}
		}

		public static IDictionary<string, (string type, string defaultValue)> GetRouteParameters(this string route)
		{
			IDictionary<string, (string type, string defaultValue)> parameters = new Dictionary<string, (string type, string defaultValue)>();

			if (route == null)
			{
				return parameters;
			}

			if (!route.EndsWith("/"))
			{
				route += "/";//Need the extra / for the regex regex parse(yes two regex)
			}

			var template = TemplateParser.Parse(route);

			return template.Parameters.ToDictionary(x => x.Name, y => (y?.InlineConstraints.FirstOrDefault()?.Constraint, y.DefaultValue?.ToString()));
		}

		public static string GetTaskType()
		{
			if (Settings.UseValueTask)
			{
				return $"ValueTask";
			}
			else
			{
				return $"Task";
			}
		}

		private static readonly Regex _attributeRegex = new Regex(@"(.+)Attribute");

		public static bool MatchesAttribute(this string str, string attribute)
		{
			str = str.Trim();
			str = str.Split('.').Last();
			attribute = attribute.Split('.').Last();

			var match = _attributeRegex.Match(attribute);
			var attributeName = match.Groups[1].Value;

			return (!string.IsNullOrEmpty(attribute) && (str.Equals(attribute) || str.Equals($"{attribute}Attribute")))
				|| (!string.IsNullOrEmpty(attributeName) && (str.Equals(attributeName) || str.Equals($"{attributeName}Attribute")));
		}

		public class TypeString
		{
			public string Name { get; set; }
			public IEnumerable<TypeString> Arguments { get; set; } = new List<TypeString>();

			public TypeString(string name)
			{
				Name = name;
			}

			public TypeString(string name, IEnumerable<TypeString> arguments)
			{
				Name = name;
				Arguments = arguments;
			}

			public override string ToString()
			{
				if (Arguments.Any())
				{
					return $"{Name}<{string.Join(", ", Arguments.Select(x => x.ToString()))}>";
				}
				else
				{
					return $"{Name}";
				}
			}
		}

		private static readonly HashSet<string> FileResults = new HashSet<string>()
		{
			typeof(PhysicalFileResult).FullName,
			typeof(FileResult).FullName,
			typeof(FileContentResult).FullName,
			typeof(FileStreamResult).FullName,
			typeof(VirtualFileResult).FullName
		};

		public static bool IsFileReturnType(this TypeString type)
		{
			return FileResults.Any(x => Helpers.IsType(x, type?.Name));
		}

		private static readonly HashSet<string> ContainerResults = new HashSet<string>()
		{
			typeof(ValueTask).FullName,
			typeof(Task).FullName,
			typeof(ActionResult).FullName
		};

		public static bool IsContainerReturnType(this TypeString type)
		{
			return ContainerResults.Any(x => Helpers.IsType(x, type?.Name));
		}


		private static readonly Regex _typeRegex = new Regex(@"(.*?)<(.*)>");

		public static TypeString GetTypeFromString(string type)
		{
			var match = _typeRegex.Match(type);
			if (!match.Success)
			{
				return new TypeString(type);
			}


			var name = match.Groups[1].Value;
			var children = match.Groups[2].Value;

			return new TypeString(name, SplitTypeParameters(children).Select(GetTypeFromString));
		}

		private static IEnumerable<string> SplitTypeParameters(string typeParameters)
		{
			List<string> parameters = new List<string>();
			StringBuilder currentParam = new StringBuilder();
			int openAngleBrakets = 0;
			foreach (var c in typeParameters)
			{
				bool ignore = false;
				if (c == ',')
				{
					if (openAngleBrakets == 0)
					{
						parameters.Add(currentParam.ToString());
						currentParam.Clear();
						ignore = true;
					}
				}
				else if (c == '<' || c == '(')//Also need to check (string,int,bool) for tuples
				{
					openAngleBrakets++;
				}
				else if (c == '>' || c == ')')
				{
					openAngleBrakets--;
				}

				if (!ignore)
				{
					currentParam.Append(c);
				}
			}

			if (currentParam.Length > 0)
			{
				parameters.Add(currentParam.ToString());
				currentParam.Clear();
			}

			return parameters;
		}

		private static readonly Regex _asyncClean = new Regex(@"^(.+)Async$");

		public static string CleanMethodName(this string str)
		{
			var match = _asyncClean.Match(str);
			if (!match.Success)
			{
				return str.Trim();
			}

			return match.Groups[1].Value.Trim();
		}

		public static bool IsType(string fullyQualifiedType, string compareType)
		{
			if (string.IsNullOrEmpty(fullyQualifiedType) && string.IsNullOrEmpty(compareType))
			{
				return true;
			}

			if (string.IsNullOrEmpty(fullyQualifiedType))
			{
				return false;
			}

			if (string.IsNullOrEmpty(compareType))
			{
				return false;
			}

			var qualified = GetTypeFromString(fullyQualifiedType);
			var compare = GetTypeFromString(compareType);

			return IsTypeString(qualified, compare);
		}

		private static bool IsTypeString(TypeString fullyQualifiedType, TypeString compareType)
		{
			if (fullyQualifiedType == null && compareType == null)
			{
				return true;
			}

			bool isType = true;

			var fq = fullyQualifiedType.Name.Split('.');
			var ct = compareType.Name.Split('.');

			isType &= !ct.Except(fq).Any();

			if (fullyQualifiedType.Arguments.Count() != compareType.Arguments.Count())
			{
				return false;
			}

			if (!fullyQualifiedType.Arguments.Any())
			{
				return isType;
			}

			for (int i = 0; i < fullyQualifiedType.Arguments.Count(); i++)
			{
				var fqArg = fullyQualifiedType.Arguments.Skip(i).Take(1).SingleOrDefault();
				var ctArg = compareType.Arguments.Skip(i).Take(1).SingleOrDefault();

				isType &= IsTypeString(fqArg, ctArg);
			}

			return isType;
		}

		public static string CleanGenericTypeDefinition(this string str)
		{
			return str.Split('`').FirstOrDefault();
		}


		public static string GetAttributeValue(this AttributeSyntax attr)
		{
			return attr.ArgumentList.Arguments.ToFullString().Replace("\"", "").Trim();
		}

		public static AttributeSyntax GetAttribute<T>(this IEnumerable<AttributeSyntax> source) where T : Attribute
		{
			return source.SingleOrDefault(x => x.Name.ToFullString().MatchesAttribute(typeof(T).Name));
		}
		public static IEnumerable<AttributeSyntax> GetAttributes<T>(this IEnumerable<AttributeSyntax> source) where T : Attribute
		{
			return source.Where(x => x.Name.ToFullString().MatchesAttribute(typeof(T).Name));
		}

		public static bool HasAttribute<T>(this IEnumerable<AttributeSyntax> source) where T : Attribute
		{
			return source.GetAttribute<T>() != null;
		}

		public static string TrimQuotes(this string str)
		{
			str = str.Trim();

			if (str.StartsWith("\"") && str.EndsWith("\""))
			{
				str = str.TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' }).Trim();
			}

			return str;
		}

		public static Dictionary<T, K> ToDictionary<T, K>(this IEnumerable<KeyValuePair<T, K>> source)
		{
			return source.ToDictionary(x => x.Key, y => y.Value);
		}
	}
}


