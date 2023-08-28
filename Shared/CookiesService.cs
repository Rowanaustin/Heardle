﻿using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
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

		public int Version { get => _version; }
		public string Guesses { get => _guesses; }
		public bool GameComplete { get => _gameComplete; }
		public int[] Results { get => _results; }
		public int LastVersionWon { get => _lastVersionWon;}
		public int Streak { get => _streak; }
		public bool DarkSetting { get => _darkSetting; set => _darkSetting = value; }

		public void Setup(ProtectedBrowserStorage localStore)
		{
			_protectedStore = localStore;
		}

		public void UpdateStorage(int version, string guesses, bool complete, int[] results, int lastVersionWon, int streak)
		{
			_version = version;
			_guesses = guesses;
			_gameComplete = complete;
			_results = results;
			_lastVersionWon = lastVersionWon;
			_streak = streak;
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
		}
	}
}