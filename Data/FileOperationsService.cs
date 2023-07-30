using System.Text;

namespace RadioHeardleServer.Data
{
	public class FileOperationsService
	{
		private readonly static string songFile = "Data/currentSong.txt";
		private readonly static string lastUpdatedFile = "Data/lastUpdated.txt";
		private readonly static string songQueueFile = "Data/songQueue.txt";
		private readonly static string songListFile = "Data/songList.txt";
		private static readonly TimeSpan serverTimeBehind = new (8, 0, 0);

		private readonly static int fileNameIndex = 0;
		private readonly static int songNameIndex = 1;

		private readonly static int updatedDateIndex = 0;
		private readonly static int versionIndex = 1;

		private readonly bool isProduction;

		private readonly string[] _songNamesList;

		public FileOperationsService(bool production)
		{
			isProduction = production;
			_songNamesList = GetSongNameList();
		}

		public DisplayData GetData()
		{
			if (NeedNewData())
				UpdateData();

			var fileName = ReadLine(songFile, fileNameIndex);
			var songName = ReadLine(songFile, songNameIndex);
			var songData = new SongData(fileName, songName);
			var updated = GetTimeUpdated();
			var version = GetVersion();

			return new DisplayData(songData, updated, version);
		}

		public List<string> GetSearchSongs(string searchStr)
		{
			return _songNamesList.Where(s => s.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase)).ToList();
		}

		private bool NeedNewData()
		{
			if (!FileExists(lastUpdatedFile))
				return true;

			if (!FileExists(songFile))
				return true;

			var songName = ReadLine(songFile, songNameIndex);
			if (songName == null || songName.Length == 0)
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

		private static string[] GetSongNameList()
		{
			var fileData = ReadLines(songListFile);

			var names = fileData.Select(d => d.Split("---")[1]).ToArray();

			return names;
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
