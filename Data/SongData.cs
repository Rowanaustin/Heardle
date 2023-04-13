namespace RadioHeardleServer.Data
{
	public struct SongData
	{
		public string _fileName;
		public string _songName;

		public SongData(string fileName, string songName)
		{
			_fileName = fileName;
			_songName = songName;
		}

		public SongData(string[] data)
		{
			_fileName = data[0];
			_songName = data[1];
		}

		public override string ToString()
		{
			return _fileName + "\n" + _songName;
		}
	}
}
