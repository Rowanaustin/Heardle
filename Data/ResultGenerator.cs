using System.Text;

namespace RadioHeardleServer.Data
{
	public static class ResultGenerator
	{
		private static readonly string whitebox = "2B1B";
		private static readonly string yellowbox = "1F7E8";
		private static readonly string greenbox = "1F7E9";

		public static string GetResultText(int guessesAllowed, string guessedOrSkipped, string edition)
		{
			StringBuilder unicodeBuilder = new StringBuilder("Radioheardle: " + edition + ": ");

			if (guessedOrSkipped.Last() == 'w')
				unicodeBuilder.Append(guessedOrSkipped.Length);
			else
				unicodeBuilder.Append('X');

			unicodeBuilder.Append("/");

			unicodeBuilder.Append(guessesAllowed);

			unicodeBuilder.Append("\n");

			if (guessedOrSkipped.Length > 0)
			{
				for (int i = 0; i < guessedOrSkipped.Length; i++)
				{
					switch (guessedOrSkipped[i])
					{
						case 'g':
							unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(yellowbox, 16)));
							break;
						case 's':
							unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(whitebox, 16)));
							break;
						default:
							unicodeBuilder.Append(char.ConvertFromUtf32(Convert.ToInt32(greenbox, 16)));
							break;
					}
				}
			}

			unicodeBuilder.Append("\n");
			unicodeBuilder.Append("http://radioheardle.homieapathy.com");

			return unicodeBuilder.ToString();
		}
	}

}
