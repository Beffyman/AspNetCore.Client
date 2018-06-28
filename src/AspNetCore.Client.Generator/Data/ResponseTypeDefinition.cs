using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;

namespace AspNetCore.Client.Generator.Data
{
	public class ResponseTypeDefinition
	{
		public MethodDefinition Parent { get; }
		public string Type { get; }
		public string Status { get; }
		public string StatusValue { get; }
		public bool IsResponseHandler { get; set; }


		public ResponseTypeDefinition(MethodDefinition parent, bool responseHandler)
		{
			Parent = parent;
			IsResponseHandler = responseHandler;
		}

		/// <summary>
		/// Internal server error
		/// </summary>
		/// <param name="parent"></param>
		public ResponseTypeDefinition(MethodDefinition parent, string expectedStatus, string type)
		{
			Parent = parent;
			Type = type;
			Status = $"(int)HttpStatusCode.{expectedStatus}";
			StatusValue = expectedStatus;
		}

		public ResponseTypeDefinition(MethodDefinition parent, AttributeSyntax attribute)
		{
			Parent = parent;

			if (attribute.ArgumentList.Arguments.Count == 1)//Only HTTP value was provided, assumed to have no body
			{
				Type = null;
				Status = attribute.ArgumentList.Arguments.SingleOrDefault().ToFullString();
			}
			else//Has 2 arguments(else invalid syntax) type,status
			{
				Type = attribute.ArgumentList.Arguments.FirstOrDefault().ToFullString().Replace("typeof", "").TrimStart('(').TrimEnd(')');
				Status = attribute.ArgumentList.Arguments.LastOrDefault().ToFullString();
			}

			if (Status.Contains("(int)"))
			{
				StatusValue = Status.Replace("(int)", "").Replace("HttpStatusCode.", "");
			}
			else
			{
				var val = int.Parse(Status);
				StatusValue = ((HttpStatusCode)val).ToString();
			}
		}



		public string ParameterName
		{
			get
			{
				if (IsResponseHandler)
				{
					return $"ResponseCallback";
				}
				return $"{StatusValue}Callback";
			}
		}

		public string SyncMethodOutput
		{

			get
			{
				if (IsResponseHandler)
				{
					return $@"Action<HttpResponseMessage> ResponseCallback = null";
				}

				if (Type == null)
				{
					return $"Action {ParameterName} = null";
				}
				else
				{
					return $"Action<{Type}> {ParameterName} = null";
				}
			}
		}

		public string AsyncMethodOutput
		{

			get
			{
				if (IsResponseHandler)
				{
					return $@"Action<HttpResponseMessage> ResponseCallback = null";
				}

				if (Type == null)
				{
					return $"Action {ParameterName} = null";
				}
				else
				{
					return $"Action<{Type}> {ParameterName} = null";
				}
			}
		}

		public string SyncMethodBlock
		{
			get
			{
				var str =
$@"			if({ParameterName} != null && {ParameterName}.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{{
				throw new NotSupportedException(""Async void action delegates for {ParameterName} are not supported. As they will run out of the scope of this call."");
			}}";


				if (IsResponseHandler)
				{
					return str += Environment.NewLine +
$@"			ResponseCallback?.Invoke(response);";
				}

				if (Type == null)
				{
					return str += Environment.NewLine +
$@"			if(response.StatusCode == System.Net.HttpStatusCode.{StatusValue})
			{{
				{StatusValue}Callback?.Invoke();
			}}";
				}
				else
				{
					return str += Environment.NewLine +
$@"			if(response.StatusCode == System.Net.HttpStatusCode.{StatusValue})
			{{
				{StatusValue}Callback?.Invoke({ReadBlock(false)});
			}}";
				}

			}
		}

		public string AsyncMethodBlock
		{
			get
			{

				var str =
$@"			if({ParameterName} != null && {ParameterName}.Method.IsDefined(typeof(AsyncStateMachineAttribute), true))
			{{
				throw new NotSupportedException(""Async void action delegates for {ParameterName} are not supported. As they will run out of the scope of this call."");
			}}";

				if (IsResponseHandler)
				{
					return str += Environment.NewLine +
$@"			ResponseCallback?.Invoke(response);";
				}

				if (Type == null)
				{
					return str += Environment.NewLine +
$@"			if(response.StatusCode == System.Net.HttpStatusCode.{StatusValue})
			{{
				{StatusValue}Callback?.Invoke();
			}}";
				}
				else
				{
					return str += Environment.NewLine +
$@"			if(response.StatusCode == System.Net.HttpStatusCode.{StatusValue})
			{{
				{StatusValue}Callback?.Invoke({ReadBlock(true)});
			}}";
				}

			}
		}


		public string ReadBlock(bool async)
		{
			if (Helpers.KnownPrimitives.Contains(Type, StringComparer.CurrentCultureIgnoreCase))
			{
				return $"{(async ? "await " : string.Empty)}response.Content.ReadAsNonJsonAsync<{Type}>(){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")}";
			}
			else
			{
				return $"{Helpers.GetJsonDeserializer()}<{Type}>({(async ? "await " : string.Empty)}response.Content.ReadAsStringAsync(){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")})";
			}
		}



		public override string ToString()
		{
			return $"{Type} {StatusValue}";
		}

	}
}
