using ProtoBuf;
using System;

namespace TestAzureFunction.Contracts
{
	[ProtoContract]
	public class User
	{
		[ProtoMember(1)]
		public string Name { get; set; }
	}
}
