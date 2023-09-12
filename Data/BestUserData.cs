namespace RadioHeardleServer.Data
{
	public class BestUserData
	{
		public int Score;
		public List<string> UserTags;

		public BestUserData(int score, List<string> userTags)
		{
			Score = score;
			UserTags = userTags;
		}
	}
}
