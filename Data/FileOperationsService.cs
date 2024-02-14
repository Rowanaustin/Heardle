using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RadioHeardleServer.Data
{
	public class FileOperationsService
	{
		private readonly static string songFile = "Data/currentSong.txt";
		private readonly static string lastUpdatedFile = "Data/lastUpdated.txt";
		private readonly static string songQueueFile = "Data/songQueue.txt";
		private readonly static string songListFile = "Data/songList.txt";
		private readonly static string bestScoreFile = "Data/bestScore.txt";
		private readonly static string bestUsersFile = "Data/bestUsers.txt";
		private static readonly TimeSpan serverTimeBehind = new (8, 0, 0);

		// last updated file
		private readonly static int updatedDateIndex = 0;
		private readonly static int versionIndex = 1;

		// best score file
		private readonly static int bestScoreIndex = 0;

		private readonly bool isProduction;

		private readonly SongData[] _songList;

		public SongData[] SongList => _songList;

		public FileOperationsService(bool production)
		{
			isProduction = production;
			_songList = GetSongList();
		}

		public DisplayData GetData()
		{
			if (NeedNewData())
				UpdateData();

			var songData = new SongData(ReadLine(songFile,0).Split("---"));
			var updated = GetTimeUpdated();
			var version = GetVersion();
			var bestScore = GetBestUserScore();
			var bestUsers = GetBestUserTags();
			var bestUserData = new BestUserData(bestScore, bestUsers);
			var queueRemaining = GetQueueLength();

			return new DisplayData(songData, updated, version, bestUserData, queueRemaining);
		}

/*		public List<string> GetSearchSongs(string searchStr)
		{
			return _songList.Where(s => s.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}*/

		public BestUserData GetBestUserData()
		{
			var currentBest = GetBestUserScore();
			var currentUsers = GetBestUserTags();

			return new BestUserData(currentBest, currentUsers);
		}

		public void PushBestScoreData(BestUserData bestUserData)
		{
			if (bestUserData.Score > 5)
				return;

			var currentBest = GetBestUserScore();

			if (bestUserData.Score == currentBest)
			{
				AppendBestScore(bestUserData);
			}
			else if (bestUserData.Score < currentBest)
			{
				ReplaceBestScore(bestUserData);
			}
		}

		private bool NeedNewData()
		{
			if (!FileExists(lastUpdatedFile))
				return true;

			if (!FileExists(songFile))
				return true;

			var fileName = ReadLine(songFile, 0).Split("---")[0];
			if (fileName == null || fileName.Length == 0)
				return true;

			return DateHasRolledOver();
		}

		private bool DateHasRolledOver()
		{
			DateTime oldDate = GetTimeUpdated();

			DateTime now = GetUkDateTime();

			// Just do it by date
			if (oldDate.Date.CompareTo(now.Date) != 0)
				return true;

			return false;
		}

		private void UpdateData()
		{
			var song = GetNextSong();

			if (song._fileName != null)
			{
				UpdateVersionFile();
				Write(songFile, song.ToString());

				ReplaceBestScore(new BestUserData(6, new List<string>()));
			}
		}

		private static SongData GetNextSong()
		{
			var songQueue = ReadLines(songQueueFile);

			if (songQueue.Length == 0 || songQueue[0].Equals(""))
				songQueue = GetNewSongQueue();

			var line = songQueue[0];
			var data = line.Split("---");

			var list = songQueue.ToList();
			list.RemoveAt(0);
			WriteLines(songQueueFile, list);

			return new SongData(data);
		}

		private static string[] GetNewSongQueue()
		{
			var songs = ReadLines(songListFile);

			var list = songs.ToList();

			Random r = new();

			return list.OrderBy(_ => r.Next()).ToArray();
		}

		private static int GetQueueLength()
		{
			var countArray = ReadLines(songQueueFile);

			return countArray.Length;
		}

		private static SongData[] GetSongList()
		{
			var fileData = ReadLines(songListFile);

			var songs = fileData.Select(d => new SongData(d.Split("---"))).ToArray();

			return songs;
		}

		private DateTime GetUkDateTime()
		{
			var time = DateTime.Now;
			if (isProduction)
				time = time.Add(serverTimeBehind);

			return time;
		}

		private DateTime GetTimeUpdated()
		{
			DateTime time;

			if (!FileExists(lastUpdatedFile))
				time = DateTime.Now.Subtract(new TimeSpan(50, 0, 0));
			else
			{
				var line = ReadLine(lastUpdatedFile, updatedDateIndex);

				if (DateTime.TryParse(line, out time))
					return time;
				else
					time = DateTime.Now.Subtract(new TimeSpan(50, 0, 0));
			}

			if (isProduction)
				time = time.Add(serverTimeBehind);

			return time;
		}

		private static int GetVersion()
		{
			if (!FileExists(lastUpdatedFile))
				return 1;
			else
			{
				var line = ReadLine(lastUpdatedFile, versionIndex);

				if (line == null || line.Trim().Length == 0)
					return 1;
				else
					return int.Parse(line.Trim());
			}
		}

		private static int GetBestUserScore()
		{
			if (!FileExists(bestScoreFile))
				return 6;
			else
			{
				var line = ReadLine(bestScoreFile, bestScoreIndex);

				if (line == null || line.Trim().Length == 0)
					return 6;
				else
					return int.Parse(line.Trim());
			}
		}

		private static List<string> GetBestUserTags()
		{
			if (!FileExists(bestScoreFile))
				return new List<string>();
			else
			{
				var lines = ReadLines(bestUsersFile);

				if (lines == null)
					return new List<string>();
				else
					return lines.Where(l => l.Trim().Length > 0).ToList();
			}
		}

		private static void AppendBestScore(BestUserData data)
		{
			var bestUsers = GetBestUserTags();

			if (bestUsers.Contains(data.UserTags[0]))
				return;

			var bestScore = GetBestUserScore();

			bestUsers.AddRange(data.UserTags);

			UpdateBestScoreFiles(bestScore.ToString(), bestUsers);
		}

		private static void ReplaceBestScore(BestUserData data)
		{
			UpdateBestScoreFiles(data.Score.ToString(), data.UserTags );
		}

		private static void UpdateBestScoreFiles(string score, List<string> users)
		{
			string usersStr = "";
			foreach(var user in users)
			{
				usersStr += user + "\n";
			}

			WriteLine(bestScoreFile, bestScoreIndex, score);
			Write(bestUsersFile, usersStr);
		}

		private void UpdateVersionFile()
		{
			var time = DateTime.Now;
			if (!FileExists(lastUpdatedFile))
				Write(lastUpdatedFile, "");

			if (isProduction)
				WriteLine(lastUpdatedFile, updatedDateIndex, string.Format("{0}", time.Add(serverTimeBehind)));
			else
				WriteLine(lastUpdatedFile, updatedDateIndex, string.Format("{0}", time).ToString());

			int version = 1;
			var versionLine = ReadLine(lastUpdatedFile, versionIndex);
			if (versionLine != null && versionLine.Length > 0)
				if (int.TryParse(versionLine.Trim(), out var fileVersion))
					version = fileVersion + 1;

			WriteLine(lastUpdatedFile, versionIndex, version.ToString());
		}

		private static bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}

		private static string[] ReadLines(string filename)
		{
			return ReadFile(filename).Split("\n");
		}

		private static string ReadLine(string filename, int index)
		{

			var lines = ReadFile(filename).Split("\n");
			if (lines.Length > index)
				return lines[index];

			return "Read nothing from line " + index + " of " + filename;
		}

		private static string ReadFile(string filename)
		{
			return File.ReadAllText(filename);
		}

		private static void Write(string filename, string text)
		{
			File.WriteAllText(filename, text);
		}

		private static void WriteLines(string filename, List<string> lines)
		{
			var builder = new StringBuilder();
			foreach(var line in lines)
			{
				builder.Append(line.Trim());
				builder.Append('\n');
			}

			Write(filename, builder.ToString());
		}

		private static void WriteLine(string filename, int index, string line)
		{
			var lines = File.ReadAllLines(filename).ToList();

			if (lines.Count < index + 1)
			{
				for (int i = lines.Count - 1; i < index; i++)
					lines.Add("");
				lines.Add(line);
			}
			else
				lines[index] = line;

			File.WriteAllLines(filename, lines);
		}
	}
}
