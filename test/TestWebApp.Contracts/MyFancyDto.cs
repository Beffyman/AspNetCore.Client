using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Contracts
{
	[ProtoContract]
	public class MyFancyDto
	{
		[ProtoMember(1)]
		public int Id { get; set; }

		[ProtoMember(2)]
		public string Description { get; set; }

		[ProtoMember(3)]
		public DateTime When { get; set; }

		[ProtoMember(4)]
		public Guid Collision { get; set; }
	}
}
