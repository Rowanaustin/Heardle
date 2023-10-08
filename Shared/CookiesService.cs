using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System.IO;
 

namespace RadioHeardleServer.Shared
{
	public class CookiesService
	{
		private ProtectedBrowserStorage _protectedStore;

		private int _version;
		private string _guesses;
		private bool _gameComplete;
		private int[] _results;
		private int _lastVersionWon;
		private int _streak;
		private bool _darkSetting;
		private string _userTag;
		private bool _dayScorePushed;
		private string _dayScorePushedWithTag;
		private string[] _songsGuessed;

		public int Version { get => _version; }
		public string Guesses { get => _guesses; }
		public bool GameComplete { get => _gameComplete; }
		public int[] Results { get => _results; }
		public int LastVersionWon { get => _lastVersionWon;}
		public int Streak { get => _streak; }
		public bool DarkSetting { get => _darkSetting; set => _darkSetting = value; }
		public string UserTag { get => _userTag; }
		public bool DayScorePushed { get => _dayScorePushed; set => _dayScorePushed = value; }
		public string[] SongsGuessed => _songsGuessed;

		public string DayScorePushedWithTag { get => _dayScorePushedWithTag; set => _dayScorePushedWithTag = value; }

		public void Setup(ProtectedBrowserStorage localStore)
		{
			_protectedStore = localStore;
		}

		public void UpdateStorage(int version, string guesses, bool complete, int[] results, int lastVersionWon, int streak, string userTag, bool dayScorePushed, List<string> songsGuessed)
		{
			_version = version;
			_guesses = guesses;
			_gameComplete = complete;
			_results = results;
			_lastVersionWon = lastVersionWon;
			_streak = streak;
			_userTag = userTag;
			_dayScorePushed = dayScorePushed;
			_songsGuessed = songsGuessed.ToArray();
		}

		public async Task LoadStorage()
		{
			try
			{
				var version = await _protectedStore.GetAsync<int>("version");
				_version = version.Success ? version.Value : 1;
				if (!version.Success)
					Console.WriteLine("version.Value == null");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("version");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}

				_version = 1;
			}

			try
			{
				var guesses = await _protectedStore.GetAsync<string>("guesses");
				_guesses = guesses.Success && guesses.Value != null ? guesses.Value : "";
				if (guesses.Value == null)
					Console.WriteLine("guesses.Value == null");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("guesses");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}
				_guesses = "";
			}

			try
			{
				var complete = await _protectedStore.GetAsync<bool>("gameComplete");
				_gameComplete = complete.Success ? complete.Value : false;
				if (!complete.Success)
					Console.WriteLine("complete.Value == null");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("gameComplete");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}
				_gameComplete = false;
			}

			try
			{
				var results = await _protectedStore.GetAsync<int[]>("results");
				_results = results.Success && results.Value != null ? results.Value : new int[6];
				if (results.Value == null)
					Console.WriteLine("results.Value == null");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("results");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}
				_results = new int[6];
			}

			try
			{
				var lastVersionCompleted = await _protectedStore.GetAsync<int>("lastVersionCompleted");
				_lastVersionWon = lastVersionCompleted.Success ? lastVersionCompleted.Value : -1;
				if (!lastVersionCompleted.Success)
					Console.WriteLine("!lastVersionCompleted.Success");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("lastVersionCompleted");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}

				_lastVersionWon = -1;
			}

			try
			{
				var streak = await _protectedStore.GetAsync<int>("streak");
				_streak = streak.Success ? streak.Value : 0;
				if (!streak.Success)
					Console.WriteLine("!streak.Success");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("streak");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}

				_streak = 0;
			}

			try
			{
				var darkMode = await _protectedStore.GetAsync<bool>("darkMode");
				DarkSetting = darkMode.Success ? darkMode.Value : false;
				if (!darkMode.Success)
					Console.WriteLine("!darkMode.Success");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("darkMode");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}
				DarkSetting = false;
			}

			try
			{
				var userTag = await _protectedStore.GetAsync<string>("userTag");
				_userTag = userTag.Success && userTag.Value != null ? userTag.Value : "";
				if (userTag.Value == null)
					Console.WriteLine("userTag.Value == null");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("userTag");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}

				_userTag = "";
			}

			try
			{
				var dayScorePushed = await _protectedStore.GetAsync<bool>("dayScorePushed");
				_dayScorePushed = dayScorePushed.Success ? dayScorePushed.Value : false;
				if (!dayScorePushed.Success)
					Console.WriteLine("dayScorePushed.Value == null");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("dayScorePushed");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}
				_dayScorePushed = false;
			}

			try
			{
				var dayScorePushedWithTag = await _protectedStore.GetAsync<string>("dayScorePushedWithTag");
				_dayScorePushedWithTag = dayScorePushedWithTag.Success && dayScorePushedWithTag.Value != null ? dayScorePushedWithTag.Value : "";
				if (dayScorePushedWithTag.Value == null)
					Console.WriteLine("dayScorePushedWithTag.Value == null");
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("dayScorePushedWithTag");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}
				_dayScorePushedWithTag = "";
			}

			try
			{
				var songsGuessed = await _protectedStore.GetAsync<string[]>("songsGuessed");
				_songsGuessed = songsGuessed.Success && songsGuessed.Value != null ? songsGuessed.Value : new string[0];
			}
			catch (Exception e)
			{
				Console.WriteLine("_protectedStore error: " + e.Message);
				try
				{
					await _protectedStore.DeleteAsync("songsGuessed");
				}
				catch (Exception iex)
				{
					Console.WriteLine("Browser storage Delete error: " + iex.Message);
				}
				_songsGuessed = new string[0];
			}
		}

		public async Task CommitStorage()
		{
			await _protectedStore.SetAsync("version", _version);
			await _protectedStore.SetAsync("guesses", _guesses);
			await _protectedStore.SetAsync("gameComplete", _gameComplete);
			await _protectedStore.SetAsync("results", _results);
			await _protectedStore.SetAsync("lastVersionCompleted", _lastVersionWon);
			await _protectedStore.SetAsync("streak", _streak);
			await _protectedStore.SetAsync("darkMode", _darkSetting);
			await _protectedStore.SetAsync("userTag", _userTag);
			await _protectedStore.SetAsync("dayScorePushed", _dayScorePushed);
			await _protectedStore.SetAsync("dayScorePushedWithTag", _dayScorePushedWithTag);
			await _protectedStore.SetAsync("songsGuessed", _songsGuessed);
		}
	}
}
