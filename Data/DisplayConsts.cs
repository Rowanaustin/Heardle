using MudBlazor;
using System.Drawing;
using static MudBlazor.Colors;

namespace RadioHeardleServer.Data
{
	public class DisplayConsts
	{
		public static Dictionary<int, AlbumColours> AlbumCovers = new Dictionary<int, AlbumColours>()
		{
			{ 0, new AlbumColours(Grey.Lighten4, Yellow.Darken2) },
			{ 1, new AlbumColours(Grey.Darken4, Brown.Lighten2) },
			{ 2, new AlbumColours(Shades.White, BlueGrey.Lighten3) },
			{ 3, new AlbumColours(BlueGrey.Darken4, Shades.White) },
			{ 4, new AlbumColours(Red.Darken1, Shades.Black) },
			{ 5, new AlbumColours(Blue.Lighten2, Red.Lighten2) },
			{ 6, new AlbumColours(Shades.Black, Orange.Lighten2) },
			{ 7, new AlbumColours(Grey.Darken4, Orange.Lighten2) },
			{ 8, new AlbumColours(Shades.Black, Grey.Lighten3) }
		};

		public static string[] AlbumNames = new string[9]
		{
			"Pablo Honey",
			"The Bends",
			"OK Computer",
			"Kid A",
			"Amnesiac",
			"Hail To The Thief",
			"In Rainbows",
			"The King of Limbs",
			"A Moon Shaped Pool"
		};
	}

	public class AlbumColours
	{
		private string _background;
		private string _highlight;

		public string Background { get => _background; }
		public string Highlight { get => _highlight; }

		public AlbumColours(string background, string highlight)
		{
			_background = background;
			_highlight = highlight;
		}
	}
}
