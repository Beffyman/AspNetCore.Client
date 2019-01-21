using System;
using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Client.Exceptions
{
	/// <summary>
	/// Thrown when a route constraint is violated during the parameter validation
	/// </summary>
	[SuppressMessage("Readability", "RCS1194", Justification = "Used by generator, only 1 constructor is used.")]
	public class InvalidRouteException : Exception
	{
		/// <summary>
		/// Throws an exception with the message provided
		/// </summary>
		/// <param name="str"></param>
		public InvalidRouteException(string str) : base(str) { }
	}
}
