using System.Diagnostics;
using System.Text;

namespace RadioHeardleServer.Data
{
	public class FileOperationsService
	{
		private static string songFile = "Data/currentSong.txt";
		private static string lastUpdatedFile = "Data/lastUpdated.txt";
		private static string songListFile = "Data/songList.txt";
		private static readonly TimeSpan updateAfter = new TimeSpan(0, 5, 0);
		private static readonly TimeSpan serverTimeBehind = new TimeSpan(7, 0, 0);

		private static int fileNameIndex = 0;
		private static int songNameIndex = 1;

		public SongData GetData()
		{
			if (NeedNewData())
				UpdateData();

			var fileName = ReadLine(songFile, fileNameIndex);
			var songName = ReadLine(songFile, songNameIndex);

			var timeUpdated = GetTimeUpdated();

			if (!Debugger.IsAttached)
				timeUpdated = timeUpdated.Add(serverTimeBehind);

			Console.WriteLine(string.Format("{0}", timeUpdated));

			return new SongData(fileName, songName);
		}

		private DateTime GetTimeUpdated()
		{
			return DateTime.Parse(ReadLine(lastUpdatedFile, 0));
		}

		private bool NeedNewData()
		{
			if (!FileExists(lastUpdatedFile))
				return true;

			DateTime oldDate = GetTimeUpdated();

			try
			{
				TimeSpan dataAge = DateTime.Now - oldDate;

				Console.WriteLine(dataAge);
				if (dataAge > updateAfter)
					return true;

				return false;
			}
			catch (Exception)
			{
				return true;
			}
		}

		private void UpdateData()
		{
			string lastUpDate = DateTime.Now.ToString();
			var song = GetRandomSong();

			if (song._fileName != null)
			{
				Write(lastUpdatedFile, lastUpDate);
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
			Console.WriteLine(line);

			if (data[0].Equals(currentSong))
			{
				line = fileData[r.Next(fileData.Length)];
				Console.WriteLine(line);
				data = line.Split("---");
			}

			return new SongData(data);
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

			Console.WriteLine("Updating index {" + index + "} of file " + filename);

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
