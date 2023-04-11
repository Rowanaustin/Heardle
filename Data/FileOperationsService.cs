using static MudBlazor.Colors;

namespace RadioHeardleServer.Data
{
	public class FileOperationsService
	{
		private static string songFile = "Data/currentSong.txt";
		private static string lastUpdatedFile = "Data/lastUpdated.txt";
		private static string songListFile = "Data/songList.txt";
		private static readonly TimeSpan updateAfter = new TimeSpan(0, 2, 0);
		private static readonly TimeSpan serverTimeBehind = new TimeSpan(8, 0, 0);

		private static int fileNameIndex = 0;
		private static int songNameIndex = 1;

		private static int updatedDateIndex = 0;
		private static int versionIndex = 1;

		private bool isProduction;

		private string[] _songNamesList;

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
			var song = GetRandomSong();

			if (song._fileName != null)
			{
				UpdateVersionFile();
				Write(songFile, song.ToString());
			}
		}

		private SongData GetRandomSong()
		{
			var currentSong = "";
			if (FileExists(songFile))
				currentSong = ReadLine(songFile, fileNameIndex);

			var fileData = ReadLines(songListFile);

			if (fileData.Length == 0)
				return new SongData();

			Random r = new();

			var line = fileData[r.Next(fileData.Length)];
			var data = line.Split("---");

			if (data[0].Equals(currentSong))
			{
				line = fileData[r.Next(fileData.Length)];
				data = line.Split("---");
			}

			return new SongData(data);
		}

		private string[] GetSongNameList()
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

				if (line.Length == 0)
					time = DateTime.Now.Subtract(new TimeSpan(50, 0, 0));
				else
					return DateTime.Parse(line);
			}

			if (isProduction)
				time = time.Add(serverTimeBehind);

			return time;
		}

		private int GetVersion()
		{
			DateTime time;

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
			if (isProduction)
				WriteLine(lastUpdatedFile, updatedDateIndex, string.Format("{0}", time.Add(serverTimeBehind)));
			else
				WriteLine(lastUpdatedFile, updatedDateIndex, string.Format("{0}", time).ToString());

			int version = 1;
			var versionLine = ReadLine(lastUpdatedFile, versionIndex);
			if (versionLine != null && versionLine.Length > 0)
				version = int.Parse(versionLine.Trim());

			WriteLine(lastUpdatedFile, versionIndex, version.ToString());
		}

		private bool FileExists(string fileName)
		{
			return File.Exists(fileName);
		}

		private string[] ReadLines(string filename)
		{
			return ReadFile(filename).Split("\n");
		}

		private string ReadLine(string filename, int index)
		{

			var lines = ReadFile(filename).Split("\n");
			if (lines.Length > index)
				return lines[index];

			return "Read nothing from line " + index + " of " + filename;
		}

		private string ReadFile(string filename)
		{
			return File.ReadAllText(filename);
		}

		private void Write(string filename, string text)
		{
			File.WriteAllText(filename, text);
		}

		private void WriteLine(string filename, int index, string line)
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
