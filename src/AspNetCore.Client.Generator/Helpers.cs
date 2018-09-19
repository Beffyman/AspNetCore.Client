using AspNetCore.Client.Generator.CSharp.Http;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AspNetCore.Client.Generator
{
	internal static class Helpers
	{
		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source) where T : class
		{
			return source.Where(x => x != null);
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

		public static string Serialize(this object obj)
		{
			return JsonConvert.SerializeObject(obj, SETTINGS);
		}

		public static T Deserialize<T>(this string str)
		{
			return JsonConvert.DeserializeObject<T>(str, SETTINGS);
		}

		public static object Deserialize(this string str, Type t)
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
		"Guid",typeof(Guid).Name,
		};

		public static bool IsRoutableType(string type)
		{
			Regex matchNullable = new Regex(@"((.+)\?)|(Nullable<(.+)>)");

			var matches = matchNullable.Matches(type);
			if (matches.Count > 0)
			{
				type = matches[matches.Count - 1].Value;
			}

			return KnownPrimitives.Contains(type, StringComparer.CurrentCultureIgnoreCase);
		}

		public static bool IsRouteParameter(string name, string fullRouteTemplate)
		{
			var routeArgs = fullRouteTemplate.GetRouteParameters();
			return routeArgs.ContainsKey(name);
		}

		public static string GetRouteStringTransform(string parameterName, string type)
		{
			var transforms = new Dictionary<string, string>
		{
			{ typeof(DateTime).Name, "{0}.ToString(\"yyyy-MM-dd HH:mm:ss\")" }
		};

			if (IsEnumerable(type))
			{
				type = GetEnumerableType(type);
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

		const string RouteParseRegex = @"{((.+?(?=:)):(.+?(?=}\/))|(.+?(?=}\/)))}";

		public static IDictionary<string, string> GetRouteParameters(this string route)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();

			if (!route.EndsWith("/"))
			{
				route += "/";//Need the extra / for the regex regex parse(yes two regex)
			}

			if (route == null)
			{
				return parameters;
			}
			var patterns = Regex.Matches(route, RouteParseRegex);

			foreach (var group in patterns)
			{
				Match match = group as Match;
				string filtered = match.Value.Replace("{", "").Replace("}", "");
				string[] split = filtered.Split(new char[] { ':' });
				if (split.Length == 1)
				{
					string variable = split[0];
					string parsedType = null;
					parameters.Add(variable, parsedType);
				}
				else
				{
					string variable = split[0];

					string type = split[1];
					parameters.Add(variable, type);
				}

			}
			return parameters;
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
			var match = _attributeRegex.Match(attribute);
			var attributeName = match.Groups[1].Value;

			return (!string.IsNullOrEmpty(attribute) && (str.Equals(attribute) || str.Equals($"{attribute}Attribute")))
				|| (!string.IsNullOrEmpty(attributeName) && (str.Equals(attributeName) || str.Equals($"{attributeName}Attribute")));
		}
	}
}
