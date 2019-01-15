using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Used for implementing custom serializers for the http content
	/// </summary>
	public interface IHttpContentSerializer
	{
		/// <summary>
		/// Content-Type that this Serializer can parse
		/// </summary>
		string ContentType { get; }

		/// <summary>
		/// Deserializes the content of the http response into the type provided
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		Task<T> Deserialize<T>(HttpContent content);

		/// <summary>
		///Serializes the request object into a string in the format of it's implementation
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		HttpContent Serialize<T>(T request);
	}
}
