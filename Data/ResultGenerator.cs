using System.Text;

namespace RadioHeardleServer.Data
{
	public static class ResultGenerator
	{
		private static readonly string whitebox = "2B1C";
		private static readonly string yellowbox = "1F7E8";
		private static readonly string greenbox = "1F7E9";

		public static string GetResultText(int guessesAllowed, List<bool> guessedOrSkipped, string edition)
		{
			StringBuilder unicodeBuilder = new StringBuilder("Radioheardle: " + edition + "\n");

			if (guessedOrSkipped.Count > 0)
			{
				for (int i = 0; i < guessedOrSkipped.Count; i++)
				{
					switch (guessedOrSkipped[i])
					{
						case true:
							unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(yellowbox, 16)));
							break;
						default:
							unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(whitebox, 16)));
							break;
					}
				}
			}

			if (guessedOrSkipped.Count < guessesAllowed)
			{
				unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(greenbox, 16)));
			}

			unicodeBuilder.Append("\n");
			unicodeBuilder.Append("http://radioheardle.homieapathy.com");

			return unicodeBuilder.ToString();
		}
	}

}
