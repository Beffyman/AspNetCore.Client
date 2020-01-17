using Beffyman.AspNetCore.Client.Generator.Framework.AspNetCoreHttp.Functions;

namespace Beffyman.AspNetCore.Client.Generator.Json
{
	public class HostJsonFile
	{
		public HostJson Data { get; }

		public HostJsonFile(string filePath)
		{
			var fileText = Helpers.SafelyReadFromFile(filePath);

			Data = Helpers.DeserializeFromJson<HostJson>(fileText);
		}
	}
}
