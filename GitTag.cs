namespace GitHelper
{
	public class GitTag
	{
		public string NAME { get; set; }
		public GitTag() { }
		public GitTag(string tag)
		{
			NAME = tag;
		}
	}
}
