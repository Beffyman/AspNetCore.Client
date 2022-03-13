using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TestWebApp.Contracts
{
	public class RequiredDto
	{
		public int Id { get; set; }
		[Required]
		public string Field1 { get; set; }
	}
}
