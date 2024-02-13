using MudBlazor;
using System.Drawing;
using static MudBlazor.Colors;

namespace RadioHeardleServer.Data
{
	public class DisplayConsts
	{
		private static string _folder = "img/";

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

		public static string[] AlbumCovers = new string[9]
		{
			_folder + "PabloHoney.png",
			_folder + "TheBends.png",
			_folder + "OKComputer.png",
			_folder + "KidA.png",
			_folder + "Amnesiac.png",
			_folder + "HailToTheThief.png",
			_folder + "InRainbows.png",
			_folder + "TheKingOfLimbs.png",
			_folder + "AMoonShapedPool.png"
		};

		public static AlbumColours[] AlbumColours = new AlbumColours[9]
		{
			new AlbumColours(Grey.Lighten4, Yellow.Darken2),
			new AlbumColours(Grey.Darken4, Brown.Lighten2),
			new AlbumColours(Shades.White, BlueGrey.Lighten3),
			new AlbumColours(BlueGrey.Darken4, Shades.White),
			new AlbumColours(Red.Darken1, Shades.Black),
			new AlbumColours(Blue.Lighten2, Red.Lighten2) ,
			new AlbumColours(Shades.Black, Orange.Lighten2),
			new AlbumColours(Grey.Darken4, Orange.Lighten2),
			new AlbumColours(Shades.Black, Grey.Lighten3)
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
