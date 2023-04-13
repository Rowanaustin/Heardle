namespace RadioHeardleServer.Data
{
	public struct DisplayData
	{
		public SongData _songData;
		public DateTime _updateDate;
		public int _version;

		public DisplayData(SongData songData, DateTime updated, int version)
		{
			_songData = songData;
			_updateDate = updated;
			_version = version;
		}
	}
}
