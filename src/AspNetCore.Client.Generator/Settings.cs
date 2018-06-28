using AspNetCore.Client.Generator.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AspNetCore.Client.Generator
{
	internal class Settings
	{
		public bool Locked { get; set; } = false;
		public string RelativeRouteToServiceProjectFolder { get; set; } = null;
		public string ClientInterfaceName { get; set; } = "MyServiceClient";
		public bool ValueTask { get; set; } = true;
		public string Namespace { get; set; } = "MyService.Clients";
		public bool BlazorClients { get; set; } = false;
		public string[] AllowedNamespaces { get; set; } = new string[]
		{
			"System*",
			"MyNamespaceToInclude*"
		};

		public string[] ExcludedNamespaces { get; set; } = new string[]
		{
			"MyNameSpaceToExclude*"
		};

		public IDictionary<string, string> KnownStatusesAndResponseTypes { get; set; }

		public IDictionary<string, IDictionary<string, string>> DefaultHttpHeaders { get; set; }

		public IDictionary<string, IDictionary<string, string>> RequiredHttpHeaderParameters { get; set; }

		[JsonIgnore]
		public IDictionary<string, List<HeaderDefinition>> RequiredHttpHeaders => RequiredHttpHeaderParameters.ToDictionary(x => x.Key, y => y.Value.Select(x => new HeaderDefinition(x.Key, x.Value, null)).ToList(), StringComparer.CurrentCultureIgnoreCase);



		public static Settings Instance { get; set; }

		public const string SettingsFileName = "ClientGeneratorSettings.json";
		public static void Load()
		{
			string path = Path.GetFullPath($"{Environment.CurrentDirectory}/{SettingsFileName}");
			if (!File.Exists(path))
			{
				Create(path);
			}

			if (File.Exists(path))
			{
				Instance = Helpers.SafelyReadFromFile(path).Deserialize<Settings>();
				NormalizeSettings();
			}
		}

		public static void Save()
		{
			NormalizeSettings();
			string path = Path.GetFullPath($"{Environment.CurrentDirectory}/{SettingsFileName}");
			Helpers.SafelyWriteToFile(path, Instance.Serialize());
		}

		public static void Create(string path)
		{
			Instance = new Settings();
			NormalizeSettings();
			Helpers.SafelyWriteToFile(path, Instance.Serialize());
		}



		public static void NormalizeSettings()
		{

			if (Instance.KnownStatusesAndResponseTypes == null)
			{
				Instance.KnownStatusesAndResponseTypes = new Dictionary<string, string>
				{
					{"BadRequest",nameof(String).ToLower() },
					{"InternalServerError",null },
				};
			}

			if (Instance.DefaultHttpHeaders == null)
			{
				Instance.DefaultHttpHeaders = new Dictionary<string, IDictionary<string, string>>
				{
					{
						"Get",
						new Dictionary<string, string>
						{
							{ "Accept","\"application/json\"" }
						}
					},
					{
						"Post",
						new Dictionary<string, string>
						{
							{ "Accept","\"application/json\"" }
						}
					},
					{
						"Put",
						new Dictionary<string, string>
						{
							{ "Accept","\"application/json\"" }
						}
					},
					{
						"Delete",
						new Dictionary<string, string>
						{
							{ "Accept","\"application/json\"" }
						}
					},
				};
			}


			if (Instance.RequiredHttpHeaderParameters == null)
			{
				Instance.RequiredHttpHeaderParameters = new Dictionary<string, IDictionary<string, string>>
				{
					{
						"Post",
						new Dictionary<string, string>
						{
							{ "UserId","int" }
						}
					},
					{
						"Put",
						new Dictionary<string, string>
						{
							{ "UserId","int" }
						}
					},
					{
						"Delete",
						new Dictionary<string, string>
						{
							{ "UserId","int" }
						}
					},
				};
			}
		}
	}
}
