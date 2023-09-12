namespace RadioHeardleServer.Data
{
	public struct DisplayData
	{
		public SongData _songData;
		public DateTime _updateDate;
		public int _version;
		public BestUserData _bestUserData;

		public DisplayData(SongData songData, DateTime updated, int version, BestUserData bestUserData)
		{
			_songData = songData;
			_updateDate = updated;
			_version = version;
			_bestUserData = bestUserData;
		}
	}
}
