using GitHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UTest
{
	[TestClass]
	public class GitTest
	{
		Git git
		{
			get
			{
				return new Git(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\..\\..\\..\\..\\");
			}
		}
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
		public void TestCommitFiles()
		{
			Commit c = git.GetCommit("74af98c8b7bad77364e46965f68a9ccbf4c0dd6c");
			if (c.EnumFiles().Count != 4)
			{
				Assert.Fail("Cannot enumerate files in commit!");
			}
		}
		[TestMethod]
		public void TestCommitFileDiff()
		{
			Commit c = git.GetCommit("74af98c8b7bad77364e46965f68a9ccbf4c0dd6c");
			if (Git.DiffFriendOutput(c.EnumFiles()[0].Diff).Count != 18)
			{
				Assert.Fail("Cannot get file history!");
			}
		}
		[TestMethod]
		public void TestCommits()
		{
			Branch b = git.GetBranch("master");
			List<Commit> ls = b.EnumCommits(1, 2);
			if (ls.Count != 2)
			{
				Assert.Fail("Cannot enumerate commits!");
			}
		}
		[TestMethod]
		public void TestTopCommit()
		{
			if (git.GetTopCommit().EnumFiles().Count < 1)
			{
				Assert.Fail("Cannot enumerate files in top commit!");
			}
		}
	}
}