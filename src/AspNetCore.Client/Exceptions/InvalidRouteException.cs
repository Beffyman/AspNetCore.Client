using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Client.Exceptions
{
	public class InvalidRouteException : Exception
	{
		public InvalidRouteException() : base() { }
		public InvalidRouteException(string str) : base(str) { }
		public InvalidRouteException(string message, Exception innerException) : base(message, innerException) { }
		protected InvalidRouteException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
