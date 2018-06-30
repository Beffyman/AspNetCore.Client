using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Contracts
{
	public class MyFancyDto
	{

		public int Id { get; set; }
		public string Description { get; set; }
		public DateTime When { get; set; }
		public Guid Collision { get; set; }

	}
}
