namespace RadioHeardleServer.Data
{
	public struct DisplayData
	{
		public SongData _songData;
		public DateTime _updateDate;

		public DisplayData(SongData songData, DateTime updated)
		{
			_songData = songData;
			_updateDate = updated;
		}
	}
}
