using System;
using System.Collections.Generic;

namespace GitHelper
{
	public class ChangedFile
	{
		Git _git;
		string _commit;
		public string Name
		{
			get; set;
		}
		public List<string> Diff
		{
			get
			{
				if (_git == null)
				{
					throw new Exception("Git is not initialized.");
				}
				if (string.IsNullOrEmpty(_commit))
				{
					throw new Exception("Commit is not initialized.");
				}
				List<string> ls = new List<string>();
				bool start = false;
				foreach (string line in _git.RunCommand(string.Format("show {0} {1}", _commit, Name)))
				{
					start = start || (line.StartsWith("@@"));
					if (!start)
					{
						continue;
					}
					ls.Add(line);
				}
				return ls;
			}
		}
		public ChangedFile() { }
		public ChangedFile(string name) { Name = name; }
		public ChangedFile(Git git, string commit, string name) { _git = git; Name = name; _commit = commit; }
	}
}