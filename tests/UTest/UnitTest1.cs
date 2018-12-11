using GitHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
		[TestMethod]
		public void TestCommits()
		{
			Git g = new Git(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
			Branch b = g.GetBranch("master");
			List<Commit> ls = b.EnumCommits(1, 2);
			if (ls.Count != 2)
			{
				Assert.Fail("Cannot enumerate commits!");
			}
		}
	}
}