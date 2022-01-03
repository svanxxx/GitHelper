using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GitHelper
{
	public class Branch
	{
		Git _git;
		public string DATE { get; set; }
		public string NAME { get; set; }
		public string AUTHOR { get; set; }
		public string AUTHOREML { get; set; }
		public string HASH { get; set; }
		public string COLOR
		{
			set { }
			get
			{
				return NAME == "master" ? "#ff00004a" : (NAME == "Release" ? "#0000ff3d" : "white");
			}
		}
		public int TTID
		{
			set { }
			get
			{
				if (NAME.StartsWith("TT"))
				{
					string s = Regex.Match(NAME, "TT[0-9]+").Value.Replace("TT", "");
					if (int.TryParse(s, out int i))
					{
						return i;
					}
				}
				return -1;
			}
		}
		public Branch() { }
		public Branch(Git git) { _git = git; }
		List<Commit> CommitsFromCommand(string command)
		{
			List<Commit> ls = new List<Commit>();
			Commit com = null;

			command += " --pretty=fuller";
			foreach (string line in _git.RunCommand(command))
			{
				if (line.StartsWith("commit"))
				{
					if (com != null)
					{
						ls.Add(com);
					}
					com = new Commit(_git);
					com.COMMIT = line.Remove(0, 7);
					com.NOTES = "";
				}
				else if (line.StartsWith("Author: "))
				{
					if (com != null)
					{
						com.AUTHOR = line.Remove(0, 8);
					}
				}
				else if (line.Contains("CommitDate:"))
				{
					if (com != null)
					{
						com.DATE = line.Split(new string[] { "CommitDate:" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
					}
				}
				else if (!string.IsNullOrEmpty(line) && line.StartsWith(" "))
				{
					if (com != null)
					{
						com.NOTES = string.IsNullOrEmpty(com.NOTES) ? line.Trim() : (com.NOTES + Environment.NewLine + line);
					}
				}
			}
			if (com != null)
			{
				ls.Add(com);
			}
			return ls;
		}
		public List<Commit> TodayCommits()
		{
			if (_git == null)
			{
				throw new Exception("Git is not initialized.");
			}
			List<Commit> ls = CommitsFromCommand($"log {NAME} --since=am");
			return ls;
		}
		public List<Commit> EnumCommits(int from, int to)
		{
			if (_git == null)
			{
				throw new Exception("Git is not initialized.");
			}
			string command = NAME == "master" ? "log -100" : string.Format(@"log master..'{0}'", NAME);
			List<Commit> ls = CommitsFromCommand(command + @" --date=format:""%Y.%m.%d %H:%M""");
			if (from > ls.Count)
			{
				return new List<Commit>();
			}
			int ifrom = Math.Min(ls.Count - 1, from - 1);
			int ito = Math.Min(ls.Count - ifrom, to - from + 1);
			return ls.GetRange(ifrom, ito);
		}
		public List<Commit> QueryCommits(string pattern)
		{
			if (_git == null)
			{
				throw new Exception("Git is not initialized.");
			}
			List<Commit> ls = CommitsFromCommand($"log -100 --grep='{pattern}'" + @" --date=format:""%Y.%m.%d %H:%M""");
			return ls;
		}
		public string TopCommit()
		{
			if (_git == null)
			{
				throw new Exception("Git is not initialized.");
			}
			string command = $"rev-parse --verify '{NAME}'";
			foreach (string line in _git.RunCommand(command))
			{
				if (line.ToUpper().Contains("FATAL"))
				{
					return "";
				}
				return line;
			}
			return "";
		}
	}
}