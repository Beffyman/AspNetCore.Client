using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Client.Serializers
{
	/// <summary>
	/// Used for implementing custom serializers for the http content
	/// </summary>
	public interface IHttpSerializer
	{
		/// <summary>
		/// Deserializes the content of the http response into the type provided
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		ValueTask<T> Deserialize<T>(HttpContent content);
		/// <summary>
		///Serializes the request object into a string in the format of it's implementation
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="request"></param>
		/// <returns></returns>
		HttpContent Serialize<T>(T request);
	}
}
