namespace RadioHeardleServer.Data
{
	public struct SongData
	{
		public string _fileName;
		public string _songName;
		public int _albumIndex;
		public int _trackIndex;

		public SongData(string fileName, string songName, int albumIndex, int trackIndex)
		{
			_fileName = fileName;
			_songName = songName;
			_albumIndex = albumIndex;
			_trackIndex = trackIndex;
		}

		public SongData(string[] data)
		{
			_fileName = data[0];
			_songName = data[1];
			_albumIndex = int.Parse(data[2]);
			_trackIndex = int.Parse(data[3]);
		}

		public override string ToString()
		{
			return _fileName + "---" + _songName + "---" + _albumIndex.ToString() + "---" + _trackIndex.ToString();
		}
	}
}
