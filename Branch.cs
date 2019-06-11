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
		public List<Commit> EnumCommits(int from, int to)
		{
			if (_git == null)
			{
				throw new Exception("Git is not initialized.");
			}
			List<Commit> ls = new List<Commit>();
			Commit com = null;
			string command = NAME == "master" ? "log -100" : string.Format(@"log master..'{0}'", NAME);

			foreach (string line in _git.RunCommand(command + @" --date=format:""%Y.%m.%d %H:%M"""))
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
				else if (line.StartsWith("Date:   "))
				{
					if (com != null)
					{
						com.DATE = line.Remove(0, 8);
					}
				}
				else if (!string.IsNullOrEmpty(line))
				{
					if (com != null)
					{
						com.NOTES = string.IsNullOrEmpty(com.NOTES) ? line : (com.NOTES + Environment.NewLine + line);
					}
				}
			}
			if (com != null)
			{
				ls.Add(com);
			}
			if (from > ls.Count)
			{
				return new List<Commit>();
			}
			int ifrom = Math.Min(ls.Count - 1, from - 1);
			int ito = Math.Min(ls.Count - ifrom, to - from + 1);
			return ls.GetRange(ifrom, ito);
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