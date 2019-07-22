using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GitHelper
{
	public class Commit
	{
		Git _git;
		public string COMMIT { get; set; }
		public string AUTHOR { get; set; }
		public string DATE { get; set; }
		public string NOTES { get; set; }
		public string NOTESFXD
		{
			get
			{
				string tt = "TT" + TTID;
				string n = NOTES.ToUpper();
				int ind = n.IndexOf(tt);
				if (ind > -1)
				{
					return NOTES.Remove(ind, tt.Length);

				}
				return NOTES;
			}
		}
		public int TTID
		{
			set { }
			get
			{
				if (NOTES.StartsWith("TT"))
				{
					string s = Regex.Match(NOTES, "TT[0-9]+").Value.Replace("TT", "");
					if (int.TryParse(s, out int i))
					{
						return i;
					}
				}
				return -1;
			}
		}
		public string AUTHORNAME
		{
			get
			{
				int sta = AUTHOR.IndexOf("<");
				int end = AUTHOR.IndexOf(">");
				if (sta < 0 || end < 0)
				{
					return AUTHOR;
				}
				return AUTHOR.Remove(sta, end - sta + 1).Trim();
			}
		}
		public string AUTHOREML
		{
			get
			{
				if (string.IsNullOrEmpty(AUTHOR))
				{
					return "";
				}
				int sta = AUTHOR.IndexOf("<");
				int end = AUTHOR.IndexOf(">");
				if (sta < 0 || end < 0)
				{
					return "";
				}
				return AUTHOR.Substring(sta + 1, end - sta - 1).Trim();
			}
		}
		public List<ChangedFile> EnumFiles()
		{
			if (_git == null)
			{
				throw new Exception("Git is not initialized.");
			}
			List<ChangedFile> ls = new List<ChangedFile>();
			string command = "show --pretty='' --name-only " + COMMIT;
			foreach (string line in _git.RunCommand(command))
			{
				ls.Add(new ChangedFile(_git, COMMIT, line.Trim()));
			}
			return ls;
		}
		public List<string> Diff()
		{
			if (_git == null)
			{
				throw new Exception("Git is not initialized.");
			}
			return _git.RunCommand("show " + COMMIT);
		}
		public Commit()
		{
		}
		public Commit(Git git)
		{
			_git = git;
		}
	}
}