using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Clients;

namespace TestWebApp.Tests
{
	[TestFixture]
	public class StaticRouteTests
	{

		[Test]
		public void TestRoute()
		{
			var actual = TestWebApp.Clients.Routes.ValuesClientRoutes.Get(5);
			var expected = "api/Values/5";
			Assert.AreEqual(expected, actual);
		}


		[Test]
		public void TestQuery()
		{
			var actual = TestWebApp.Clients.Routes.ValuesClientRoutes.EnumerableGet(new List<int> { 1, 2, 3, 4 }, new List<bool> { true, false, true });
			var expected = "api/Values/EnumerableGet?ids=1&ids=2&ids=3&ids=4&truth=True&truth=False&truth=True";
			Assert.AreEqual(expected, actual);
		}
	}
}
