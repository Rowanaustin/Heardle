namespace RadioHeardleServer.Data
{
	public class FileOperationsService
	{
		private static string songFile = "Data/currentSong.txt";
		private static string lastUpdatedFile = "Data/lastUpdated.txt";
		private static string songListFile = "Data/songList.txt";
		private static readonly TimeSpan updateAfter = new TimeSpan(1, 0, 0);
		private static readonly TimeSpan serverTimeBehind = new TimeSpan(0, 15, 0);

		private static int fileNameIndex = 0;
		private static int songNameIndex = 1;

		private bool isProduction;

		public FileOperationsService(bool production)
		{
			isProduction = production;
		}

		public DisplayData GetData()
		{
			if (NeedNewData())
				UpdateData();

			var fileName = ReadLine(songFile, fileNameIndex);
			var songName = ReadLine(songFile, songNameIndex);
			var songData = new SongData(fileName, songName);
			var updated = GetTimeUpdated();

			return new DisplayData(songData, updated);
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


			//try
			//{
			//	TimeSpan dataAge = DateTime.Now - oldDate;

			//	if (dataAge > updateAfter)
			//		return true;

			//	return false;
			//}
			//catch (Exception)
			//{
			//	return true;
			//}
		}

		private void UpdateData()
		{
			var song = GetRandomSong();

			if (song._fileName != null)
			{
				SetTimeUpdated();
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
				var line = ReadLine(lastUpdatedFile, 0);

				if (line.Length == 0)
					time = DateTime.Now.Subtract(new TimeSpan(50, 0, 0));
				else
					return DateTime.Parse(line);
			}

			if (isProduction)
				time = time.Add(serverTimeBehind);

			return time;
		}

		private void SetTimeUpdated()
		{
			var time = DateTime.Now;
			if (isProduction)
				Write(lastUpdatedFile, string.Format("{0}", time.Add(serverTimeBehind)));
			else
				Write(lastUpdatedFile, string.Format("{0}", time).ToString());
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
