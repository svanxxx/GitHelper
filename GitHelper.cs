using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace GitHelper
{
	public class Git
	{
		string _path;
		public Git(string path)
		{
			_path = path;
		}
		public void SetCredentials(string user, string email)
		{
			RunCommand(string.Format("config --local user.name \"{0}\"", user));
			RunCommand(string.Format("config --local user.email \"{0}\"", email));
		}
		public List<string> ResetHard(string tobranch = "")
		{
			return RunCommand("reset --hard " + tobranch);
		}
		public List<string> Status()
		{
			return RunCommand("status");
		}
		public List<string> CleanaupLocalTags()
		{
			return RunCommand("fetch --prune origin \"+refs/tags/*:refs/tags/*\"");
		}
		public List<string> AddTag(string tag)
		{
			return RunCommand($"tag {tag}");
		}
		public List<GitTag> EnumTags()
		{
			List<GitTag> tags = new List<GitTag>();
			foreach (string t in RunCommand("tag"))
			{
				tags.Add(new GitTag(t));
			}
			return tags;
		}
		public List<string> DeleteAllTags()
		{
			return RunCommand("tag | foreach-object -process { git.exe tag -d $_ }");
		}
		public List<string> PushTags()
		{
			return RunCommand("push --tags");
		}
		public List<string> FetchAll()
		{
			return RunCommand("fetch --all");
		}
		public List<string> PullOrigin()
		{
			return RunCommand("pull origin");
		}
		public List<string> PushOrigin()
		{
			return RunCommand("push origin HEAD");
		}
		public string CurrentBranch()
		{
			return RunCommand("rev-parse --abbrev-ref HEAD")[0];
		}
		public List<string> AddFile(string file)
		{
			return RunCommand(string.Format("add -f \"{0}\"", file));
		}
		public List<string> CommitAll(string message, string author)
		{
			return RunCommand(string.Format("commit --all --message=\"{0}\" --author=\"{1}\"", message, author));
		}
		public List<string> PushCurrentBranch()
		{
			return RunCommand(string.Format("push origin refs/heads/{0}:refs/heads/{0}", CurrentBranch()));
		}
		public List<string> Rebase(string barnch)
		{
			return RunCommand("rebase " + barnch);
		}
		public List<string> Checkout(string barnch)
		{
			return RunCommand("checkout " + barnch);
		}
		public List<string> RunCommand(string command)
		{
			using (PowerShell ps = PowerShell.Create())
			{
				ps.AddScript("cd " + _path);
				ps.AddScript(@"git.exe " + command);
				List<string> ls = new List<string>();
				foreach (var line in ps.Invoke())
				{
					ls.Add(line.ToString());
				}
				return ls;
			}
		}
		public List<string> Diff()
		{
			return RunCommand("diff HEAD^ HEAD");
		}
		static public List<string> DiffFriendOutput(List<string> res)
		{
			List<string> processed = new List<string>();
			for (int i = 0; i < res.Count; i++)
			{
				string s = res[i].Replace("<", "&lt;").Replace(">", "&gt;");
				string style = "";
				string pre = "";
				string pos = "";
				if (s.StartsWith("+++ b"))
				{
					continue;
				}
				else if (s.StartsWith("--- a"))
				{
					continue;
				}
				if (s.StartsWith("+"))
				{
					style = " style='background-color:#dfd;'";
				}
				else if (s.StartsWith("-"))
				{
					style = " style='background-color:#fdd;'";
				}
				else if (s.StartsWith("@@"))
				{
					style = " style='background-color:#80808042;'";
				}
				else if (s.StartsWith("fatal"))
				{
					style = " style='background-color:red;'";
				}
				else if (s.StartsWith("diff --git a"))
				{
					s = "<hr>" + s.Replace("diff --git a", "").Split(new string[] { "b/" }, StringSplitOptions.RemoveEmptyEntries)[0];
					pre = "<b>";
					pos = "</b>";
				}
				processed.Add(string.Format("<span{0}>{2}{1}{3}</span>", style, s, pre, pos));
			}
			return processed;
		}

		static object _lock = new object();
		static DateTime _loadtime = DateTime.Now;
		static List<Branch> _branches = new List<Branch>();
		public List<Branch> EnumBranches(string user)
		{
			lock (_lock)
			{
				if (_branches.Count < 1 || (DateTime.Now - _loadtime).TotalSeconds > 20) //cached by 20 seconds span to reduce disk load and response time
				{
					_branches.Clear();
					_loadtime = DateTime.Now;
					foreach (string line in RunCommand(@"for-each-ref --format=""%(committerdate) %09 %(authorname) %09 %(refname) %09 %(authoremail)"" --sort=-committerdate"))
					{
						string[] sep = new string[] { "\t" };
						string[] pars = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
						if (pars.Length == 4)
						{
							_branches.Add(new Branch(this) { DATE = pars[0].Trim(), AUTHOR = pars[1].Trim(), NAME = pars[2].Trim().Split('/')[2], AUTHOREML = pars[3].Trim('>', '<', ' ', '\t') });
						}
					}
				}
				if (string.IsNullOrEmpty(user))
				{
					return new List<Branch>(_branches);
				}
				return new List<Branch>(_branches.FindAll(s => s.AUTHOR == user));
			}
		}
		public List<Branch> EnumBranches(int from, int to, string user)
		{
			List<Branch> ls = EnumBranches(user);
			int count = to - from + 1;
			if (ls.Count < 1 || from < 1 || from > ls.Count || count < 1)
			{
				return new List<Branch>();
			}
			count = Math.Min(count, ls.Count - from + 1);
			return ls.GetRange(from - 1, count);
		}
		public void DeleteBranch(string branch)
		{
			RunCommand(string.Format("branch -D {0}", branch));
		}
		public Branch GetBranch(string branch)
		{
			return new Branch(this){NAME = branch};
		}
		public Commit GetCommit(string commit)
		{
			return new Commit(this) { COMMIT = commit };
		}
		public Commit GetTopCommit()
		{
			return new Commit(this) { COMMIT = RunCommand("rev-parse HEAD")[0] };
		}
	}
}