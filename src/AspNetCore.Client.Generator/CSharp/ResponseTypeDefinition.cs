using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using AspNetCore.Client.Serializers;

namespace AspNetCore.Client.Generator.CSharp
{
	public class ResponseTypeDefinition
	{
		public string Type { get; }
		public string Status { get; }
		public string StatusValue { get; }
		public bool IsResponseHandler { get; set; }


		public ResponseTypeDefinition(bool responseHandler)
		{
			IsResponseHandler = responseHandler;
		}


		public ResponseTypeDefinition(string expectedStatus, string type)
		{
			Type = type;
			Status = $"(int){nameof(HttpStatusCode)}.{expectedStatus}";
			StatusValue = expectedStatus;
		}

		public ResponseTypeDefinition(AttributeSyntax attribute)
		{
			if (attribute.ArgumentList.Arguments.Count == 1)//Only HTTP value was provided, assumed to have no body
			{
				Type = null;
				Status = attribute.ArgumentList.Arguments.SingleOrDefault().ToFullString();
			}
			else//Has 2 arguments(else invalid syntax) type,status
			{
				Type = attribute.ArgumentList.Arguments.FirstOrDefault().ToFullString().Replace("typeof", "").Trim().TrimStart('(').TrimEnd(')');
				Status = attribute.ArgumentList.Arguments.LastOrDefault().ToFullString();
			}

			if (Type != null)
			{
				if (Type.Contains("StatusCodes.")
				|| Type.Contains("(int)")
				|| Type.Contains($"{ nameof(HttpStatusCode)}."))
				{
					if (Status?.Contains("=") ?? false)
					{
						Status = Status.Split('=')[1].Trim();
					}


					var tmp = Type;
					Type = Status.Replace("typeof", "").Trim().TrimStart('(').TrimEnd(')');
					Status = tmp;
				}
			}

			if (Type?.Contains("=") ?? false)
			{
				Type = Type.Split('=')[1].Trim();
			}


			if (Status.Contains("(int)"))
			{
				StatusValue = Status.Replace("(int)", "").Replace("StatusCodes.Status", "").Replace($"{nameof(HttpStatusCode)}.", "");
			}
			else if (Status.Contains("StatusCodes.Status"))
			{
				StatusValue = Status.Replace("(int)", "").Replace("StatusCodes.Status", "").Replace($"{nameof(HttpStatusCode)}.", "");
				Status = StatusValue = new string(StatusValue.Where(x => !char.IsNumber(x)).ToArray());
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
					return $"{Constants.ResponseCallback}";
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
					return $@"Action<HttpResponseMessage> {Constants.ResponseCallback} = null";
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
					return $@"Action<HttpResponseMessage> {Constants.ResponseCallback} = null";
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
$@"			{Constants.ResponseCallback}?.Invoke(response);";
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
$@"			{Constants.ResponseCallback}?.Invoke(response);";
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
			return $"{(async ? "await " : string.Empty)}{Constants.SerializerField}.{nameof(IHttpSerializer.Deserialize)}<{Type}>({Constants.ResponseVariable}.Content){(async ? ".ConfigureAwait(false)" : ".ConfigureAwait(false).GetAwaiter().GetResult()")}";
		}



		public override string ToString()
		{
			return $"{Type} {StatusValue}";
		}

	}
}
