using GitHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTest
{
	[TestClass]
	public class GitTest
	{
		[TestMethod]
		public void TestBranchTT()
		{
			Branch b = new Branch(null);
			b.NAME = "TT";
			if (b.TTID != -1)
			{
				Assert.Fail("invalid process of branch text");
			}
			b.NAME = "TT1";
			if (b.TTID != 1)
			{
				Assert.Fail("invalid process of branch text");
			}
			b.NAME = "TT123";
			if (b.TTID != 123)
			{
				Assert.Fail("invalid process of branch text");
			}
			b.NAME = "TT123hiall";
			if (b.TTID != 123)
			{
				Assert.Fail("invalid process of branch text");
			}
			b.NAME = "TT123_hiall123455";
			if (b.TTID != 123)
			{
				Assert.Fail("invalid process of branch text");
			}
		}
	}
}